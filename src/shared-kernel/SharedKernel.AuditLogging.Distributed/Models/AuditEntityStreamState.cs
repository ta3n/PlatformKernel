namespace SharedKernel.AuditLogging.Distributed.Models;

public class AuditEntityStreamState
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string SourceService { get; set; } = string.Empty;

    public string EntityName { get; set; } = string.Empty;

    public string EntityKey { get; set; } = string.Empty;

    public long LastProcessedVersion { get; set; }

    public DateTimeOffset UpdatedUtc { get; set; } = DateTimeOffset.UtcNow;
}
