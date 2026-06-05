using SharedKernel.AuditLogging.Models;

namespace SharedKernel.AuditLogging.Distributed.Contracts;

public sealed class AuditEntityChanged
{
    public Guid EventId { get; init; } = Guid.NewGuid();

    public string SourceService { get; init; } = string.Empty;

    public string EntityName { get; init; } = string.Empty;

    public string EntityKey { get; init; } = string.Empty;

    public long? EntityVersion { get; init; }

    public AuditOperation Operation { get; init; }

    public DateTimeOffset OccurredUtc { get; init; } = DateTimeOffset.UtcNow;

    public string UserId { get; init; } = "SYSTEM";

    public string UserName { get; init; } = "SYSTEM";

    public string? TenantId { get; init; }

    public string? IpAddress { get; init; }

    public string? TraceId { get; init; }

    public string BeforeJson { get; init; } = "{}";

    public string AfterJson { get; init; } = "{}";

    public string MetadataJson { get; init; } = "{}";

    public int SchemaVersion { get; init; } = 1;
}
