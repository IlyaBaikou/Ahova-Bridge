using System.Text.Json;
using Ahova.Bridge.Administration;

namespace Ahova.Bridge.Capabilities.Ai;

internal sealed class LocalAiDiscoveryService(HttpClient httpClient)
{
    private const int MaximumModels = 100;
    private const int MaximumModelNameLength = 200;

    public async Task<LocalAiDiscoveryResult> DiscoverAsync(
        DiscoverLocalAiRequest request, CancellationToken cancellationToken)
    {
        var candidates = Candidates(request).Distinct().ToArray();
        var probes = candidates.Select(candidate => ProbeAsync(candidate, cancellationToken));
        var results = await Task.WhenAll(probes);
        return new(results.Where(result => result is not null)
            .Select(result => result!).ToArray());
    }

    private async Task<DiscoveredLocalAi?> ProbeAsync(AiCandidate candidate,
        CancellationToken cancellationToken)
    {
        try
        {
            var endpoint = candidate.Provider == "ollama"
                ? OllamaEndpoint(candidate.BaseUrl, "api/tags")
                : Endpoint(candidate.BaseUrl, "models");
            using var response = await httpClient.GetAsync(endpoint, cancellationToken);
            if (!response.IsSuccessStatusCode) return null;
            await using var content = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var document = await JsonDocument.ParseAsync(content,
                cancellationToken: cancellationToken);
            var models = candidate.Provider == "ollama"
                ? OllamaModels(document.RootElement)
                : OpenAiModels(document.RootElement);
            return new(candidate.Provider, candidate.BaseUrl, models);
        }
        catch (Exception exception) when (exception is HttpRequestException
            or TaskCanceledException or JsonException or InvalidOperationException
            or UriFormatException)
        {
            return null;
        }
    }

    private static IEnumerable<AiCandidate> Candidates(DiscoverLocalAiRequest request)
    {
        var provider = NormalizeProvider(request.Provider);
        if (!string.IsNullOrWhiteSpace(request.BaseUrl)
            && Uri.TryCreate(request.BaseUrl.Trim(), UriKind.Absolute, out var requested)
            && requested.Scheme is "http" or "https")
        {
            yield return new(provider == "ollama" ? "ollama" : "openai-compatible",
                EnsureTrailingSlash(requested.AbsoluteUri));
        }

        if (provider is "auto" or "ollama")
            yield return new("ollama", "http://host.docker.internal:11434/v1/");
        if (provider is "auto" or "openai-compatible")
            yield return new("openai-compatible", "http://host.docker.internal:1234/v1/");
    }

    private static string[] OllamaModels(JsonElement root) =>
        ReadModels(root, "models", "name", "model");

    private static string[] OpenAiModels(JsonElement root) =>
        ReadModels(root, "data", "id");

    private static string[] ReadModels(JsonElement root, string collection,
        params string[] properties)
    {
        if (!root.TryGetProperty(collection, out var values)
            || values.ValueKind != JsonValueKind.Array) return [];
        return values.EnumerateArray().Select(value => properties.Select(property =>
                value.TryGetProperty(property, out var item) && item.ValueKind == JsonValueKind.String
                    ? item.GetString() : null).FirstOrDefault(item => !string.IsNullOrWhiteSpace(item)))
            .Where(item => item is not null && item.Length <= MaximumModelNameLength)
            .Select(item => item!)
            .Distinct(StringComparer.Ordinal)
            .Take(MaximumModels)
            .ToArray();
    }

    private static string NormalizeProvider(string? provider) =>
        provider?.Trim().ToLowerInvariant() switch
        {
            "ollama" => "ollama",
            "openai-compatible" or "lm-studio" => "openai-compatible",
            _ => "auto",
        };

    private static string EnsureTrailingSlash(string value) => value.TrimEnd('/') + "/";

    private static Uri Endpoint(string baseUrl, string path) =>
        new(new Uri(EnsureTrailingSlash(baseUrl), UriKind.Absolute), path);

    private static Uri OllamaEndpoint(string baseUrl, string path)
    {
        var source = new Uri(EnsureTrailingSlash(baseUrl), UriKind.Absolute);
        var builder = new UriBuilder(source);
        var rootPath = builder.Path.TrimEnd('/');
        if (rootPath.EndsWith("/v1", StringComparison.OrdinalIgnoreCase))
            rootPath = rootPath[..^3];
        builder.Path = $"{rootPath.TrimEnd('/')}/{path.TrimStart('/')}";
        builder.Query = string.Empty;
        builder.Fragment = string.Empty;
        return builder.Uri;
    }

    private sealed record AiCandidate(string Provider, string BaseUrl);
}
