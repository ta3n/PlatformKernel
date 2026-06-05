namespace SharedKernel.AuditLogging.Models;

public sealed class AuditContextSnapshot
{
    public string UserId { get; init; } = "SYSTEM";

    public string UserName { get; init; } = "SYSTEM";

    public string? TenantId { get; init; }

    public string? IpAddress { get; init; }

    public string? TraceId { get; init; }

    public string Source { get; init; } = "Application";
}
