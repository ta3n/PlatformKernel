namespace SharedKernel.AuditLogging.Distributed.Models;

public class AuditPendingEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid EventId { get; set; }

    public string SourceService { get; set; } = string.Empty;

    public string EntityName { get; set; } = string.Empty;

    public string EntityKey { get; set; } = string.Empty;

    public long EntityVersion { get; set; }

    public string PayloadJson { get; set; } = string.Empty;

    public DateTimeOffset ReceivedUtc { get; set; } = DateTimeOffset.UtcNow;
}
