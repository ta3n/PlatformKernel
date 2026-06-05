namespace SharedKernel.AuditLogging.Distributed.Options;

public sealed class AuditServiceOptions
{
    public const string SectionName = "AuditService";

    public bool RequireEntityVersion { get; set; } = true;

    public bool AllowFirstVersionToStartAtAnyValue { get; set; }

    public bool SkipEmptyChanges { get; set; } = true;
}
