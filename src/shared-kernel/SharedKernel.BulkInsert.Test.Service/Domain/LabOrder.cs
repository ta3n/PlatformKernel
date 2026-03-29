namespace SharedKernel.BulkInsert.Test.Service.Domain;

public sealed class LabOrder
{
    public long Id { get; set; }

    public required string ExternalId { get; set; }

    public int Quantity { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public required string Source { get; set; }
}
