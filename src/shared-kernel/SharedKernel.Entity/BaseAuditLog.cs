namespace SharedKernel.Entity;

public class BaseAuditLog : EntityData
{
    public string? EventType { get; set; }

    public string? AggregateCode { get; set; }

    public string? Request { get; set; }

    public string? ChangedFields { get; set; }

    public string? UserCode { get; set; }
}
