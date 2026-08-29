using System.Net;
using System.Text;
using System.Text.Json;
using Ahova.Bridge.Capabilities.Ai;
using Ahova.Bridge.Configuration;
using Microsoft.Extensions.Options;

namespace Ahova.Bridge.Tests;

public sealed class LocalAiClientTests : IDisposable
{
    private readonly string stateDirectory = Path.Combine(Path.GetTempPath(),
        $"ahova-local-ai-tests-{Guid.NewGuid():N}");

    [Fact]
    public async Task OllamaUsesNativeJsonModeWithoutCompilingTheResponseSchema()
    {
        var handler = new RecordingHandler(_ => Json(HttpStatusCode.OK,
            "{\"message\":{\"role\":\"assistant\",\"content\":\"{\\\"answer\\\":\\\"ok\\\"}\"},\"done\":true}"));
        using var configuration = Configuration("ollama", "http://localhost:11434/v1/");
        var client = new OpenAiCompatibleLocalAiClient(new HttpClient(handler), configuration);
        using var schema = JsonDocument.Parse("""
            {"type":"object","properties":{"answer":{"type":"string","maxLength":2000},"sourceKeys":{"type":"array","maxItems":8,"items":{"type":"string"}}},"required":["answer"]}
            """);

        var result = await client.CompleteAsync(Request(schema.RootElement.Clone()),
            CancellationToken.None);

        Assert.Equal("{\"answer\":\"ok\"}", result.Json);
        var sent = Assert.Single(handler.Requests);
        Assert.Equal("/api/chat", sent.Uri.AbsolutePath);
        using var body = JsonDocument.Parse(sent.Body);
        var format = body.RootElement.GetProperty("format");
        Assert.Equal(JsonValueKind.Object, format.ValueKind);
        Assert.Equal("object", format.GetProperty("type").GetString());
        Assert.False(format.GetProperty("properties").GetProperty("answer")
            .TryGetProperty("maxLength", out _));
        Assert.Equal(8, format.GetProperty("properties").GetProperty("sourceKeys")
            .GetProperty("maxItems").GetInt32());
        Assert.False(body.RootElement.TryGetProperty("response_format", out _));
        Assert.Contains("maxLength", body.RootElement.GetProperty("messages")[0]
            .GetProperty("content").GetString(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task GenericProviderKeepsOpenAiJsonSchemaContract()
    {
        var handler = new RecordingHandler(_ => Json(HttpStatusCode.OK,
            "{\"choices\":[{\"message\":{\"content\":\"{\\\"answer\\\":\\\"ok\\\"}\"}}]}"));
        using var configuration = Configuration("lm-studio", "http://localhost:1234/v1/");
        var client = new OpenAiCompatibleLocalAiClient(new HttpClient(handler), configuration);
        using var schema = JsonDocument.Parse(
            "{\"type\":\"object\",\"properties\":{\"answer\":{\"type\":\"string\"}}}");

        await client.CompleteAsync(Request(schema.RootElement.Clone()), CancellationToken.None);

        var sent = Assert.Single(handler.Requests);
        Assert.Equal("/v1/chat/completions", sent.Uri.AbsolutePath);
        using var body = JsonDocument.Parse(sent.Body);
        Assert.Equal("json_schema", body.RootElement.GetProperty("response_format")
            .GetProperty("type").GetString());
    }

    [Fact]
    public async Task DiscoveryFindsOllamaAndReturnsBoundedModelNames()
    {
        var handler = new RecordingHandler(request => request.RequestUri?.AbsolutePath == "/api/tags"
            ? Json(HttpStatusCode.OK,
                "{\"models\":[{\"name\":\"qwen3:8b\"},{\"model\":\"gemma3:4b\"}]}")
            : Json(HttpStatusCode.NotFound, "{}"));
        var discovery = new LocalAiDiscoveryService(new HttpClient(handler));

        var result = await discovery.DiscoverAsync(
            new Administration.DiscoverLocalAiRequest("ollama"), CancellationToken.None);

        var server = Assert.Single(result.Servers);
        Assert.Equal("ollama", server.Provider);
        Assert.Equal(["qwen3:8b", "gemma3:4b"], server.Models);
        Assert.EndsWith("/api/tags", Assert.Single(handler.Requests).Uri.AbsolutePath,
            StringComparison.Ordinal);
    }

    [Fact]
    public async Task DiscoveryFindsOpenAiCompatibleModels()
    {
        var handler = new RecordingHandler(_ => Json(HttpStatusCode.OK,
            "{\"data\":[{\"id\":\"local-model\"}]}"));
        var discovery = new LocalAiDiscoveryService(new HttpClient(handler));

        var result = await discovery.DiscoverAsync(
            new Administration.DiscoverLocalAiRequest("openai-compatible",
                "http://localhost:1234/v1/"), CancellationToken.None);

        Assert.Contains(result.Servers, server => server.Provider == "openai-compatible"
            && server.Models.SequenceEqual(["local-model"]));
    }

    private BridgeRuntimeConfiguration Configuration(string provider, string baseUrl) => new(
        Options.Create(new BridgeOptions
        {
            StateDirectory = stateDirectory,
            ControlPlaneBaseUrl = "https://api.example.test",
            Ai = new LocalAiOptions
            {
                Enabled = true,
                Provider = provider,
                BaseUrl = baseUrl,
                AllowedModels = ["test-model"],
            },
        }));

    private static LocalAiJobRequest Request(JsonElement schema) => new(
        "ai.openai-compatible", "test-model", "Answer from the supplied source.",
        "What is covered?", schema, 512, 30);

    private static HttpResponseMessage Json(HttpStatusCode status, string body) => new(status)
    {
        Content = new StringContent(body, Encoding.UTF8, "application/json"),
    };

    public void Dispose()
    {
        if (Directory.Exists(stateDirectory)) Directory.Delete(stateDirectory, true);
    }

    private sealed class RecordingHandler(
        Func<HttpRequestMessage, HttpResponseMessage> responseFactory) : HttpMessageHandler
    {
        public List<RecordedRequest> Requests { get; } = [];

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var body = request.Content is null ? string.Empty
                : await request.Content.ReadAsStringAsync(cancellationToken);
            Requests.Add(new(request.RequestUri!, body));
            return responseFactory(request);
        }
    }

    private sealed record RecordedRequest(Uri Uri, string Body);
}
