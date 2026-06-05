namespace SharedKernel.AuditLogging.Distributed.Options;

public sealed class AuditProducerOptions
{
    public const string SectionName = "AuditProducer";

    public bool Enabled { get; set; } = true;

    public string SourceService { get; set; } = "Application";

    public bool IncludeShadowProperties { get; set; }

    public string RedactedValue { get; set; } = "***REDACTED***";

    public bool RequireStableEntityKey { get; set; } = true;

    public bool RequireEntityVersion { get; set; }

    public int SchemaVersion { get; set; } = 1;
}
