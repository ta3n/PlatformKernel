using SharedKernel.AuditLogging.Models;

namespace SharedKernel.AuditLogging.Options;

public sealed class AuditLoggingOptions
{
    public const string SectionName = "AuditLogging";

    public bool Enabled { get; set; } = true;

    public AuditLoggingMode Mode { get; set; } = AuditLoggingMode.Sync;

    public bool IncludeShadowProperties { get; set; }

    public bool IncludeUnchangedOwnedTypes { get; set; }

    public string RedactedValue { get; set; } = "***REDACTED***";

    public string Source { get; set; } = "Application";

    public bool EnableHashChain { get; set; }

    public int OutboxBatchSize { get; set; } = 100;

    public TimeSpan OutboxPollingInterval { get; set; } = TimeSpan.FromSeconds(5);
}
