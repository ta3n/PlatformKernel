namespace SharedKernel.AuditLogging.Models;

public class AuditOutbox
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string PayloadJson { get; set; } = string.Empty;

    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    public DateTime? ProcessedUtc { get; set; }

    public int Attempts { get; set; }

    public string? LastError { get; set; }
}
