using System.Reflection;
using Ahova.Bridge.Configuration;
using Ahova.Bridge.ControlPlane;
using Ahova.Bridge.Security;
using Ahova.Bridge.Capabilities.Ai;
using Ahova.Bridge.Capabilities.Storage;

namespace Ahova.Bridge.Runtime;

public sealed class BridgeRuntimeState(
    IInstallationCredentialStore credentialStore,
    BridgeRuntimeConfiguration configuration,
    ILocalAiClient aiClient,
    ILocalStorageClient storageClient)
{
    private string? lastSafeFailure;
    private long lastControlPlaneContactUnixMilliseconds;

    public IReadOnlyList<string> Capabilities
    {
        get
        {
            var settings = configuration.Snapshot();
            var capabilities = new List<string>();
            if (settings.AiEnabled) capabilities.Add("ai.openai-compatible");
            if (settings.StorageEnabled)
            {
                capabilities.Add("storage.stat");
                capabilities.Add("storage.read");
                if (!settings.StorageReadOnly)
                {
                    capabilities.Add("storage.write");
                    capabilities.Add("storage.delete");
                }
            }
            return capabilities;
        }
    }

    public string Version => Assembly.GetExecutingAssembly().GetName().Version?.ToString()
        ?? "0.0.0";

    public void ReportFailure(string safeCode) =>
        Volatile.Write(ref lastSafeFailure, safeCode);

    public void ReportHealthy() => Volatile.Write(ref lastSafeFailure, null);

    public void ReportControlPlaneContact(DateTimeOffset contactedAt) =>
        Interlocked.Exchange(ref lastControlPlaneContactUnixMilliseconds,
            contactedAt.ToUnixTimeMilliseconds());

    public async Task<BridgeRuntimeStatus> SnapshotAsync(CancellationToken cancellationToken)
    {
        var credential = await credentialStore.ReadAsync(cancellationToken);
        var settings = configuration.Snapshot();
        var aiHealth = !settings.AiEnabled ? "disabled"
            : await aiClient.IsHealthyAsync(cancellationToken) ? "healthy" : "unavailable";
        var storageHealth = !settings.StorageEnabled ? "disabled"
            : await storageClient.IsHealthyAsync(cancellationToken) ? "healthy" : "unavailable";
        var lastContact = Interlocked.Read(ref lastControlPlaneContactUnixMilliseconds);
        return new BridgeRuntimeStatus(
            credential is not null,
            credential?.InstallationId,
            !string.IsNullOrWhiteSpace(configuration.Snapshot().ControlPlaneBaseUrl),
            Capabilities,
            Version,
            Volatile.Read(ref lastSafeFailure),
            aiHealth,
            storageHealth,
            lastContact == 0 ? null : DateTimeOffset.FromUnixTimeMilliseconds(lastContact));
    }
}
