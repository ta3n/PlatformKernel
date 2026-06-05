namespace SharedKernel.AuditLogging.Models;

public class AuditLog
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string EntityName { get; set; } = string.Empty;

    public string EntityId { get; set; } = string.Empty;

    public AuditOperation Operation { get; set; }

    public string UserId { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string? TenantId { get; set; }

    public string? IpAddress { get; set; }

    public string? TraceId { get; set; }

    public string Source { get; set; } = string.Empty;

    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;

    public string ChangesJson { get; set; } = "{}";

    public string MetadataJson { get; set; } = "{}";

    public string? PreviousHash { get; set; }

    public string? Hash { get; set; }
}
