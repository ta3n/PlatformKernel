namespace SharedKernel.AuditLogging.Distributed.Models;

public class AuditProcessedEvent
{
    public Guid EventId { get; set; }

    public string SourceService { get; set; } = string.Empty;

    public string EntityName { get; set; } = string.Empty;

    public string EntityKey { get; set; } = string.Empty;

    public long? EntityVersion { get; set; }

    public DateTimeOffset ProcessedUtc { get; set; } = DateTimeOffset.UtcNow;
}
