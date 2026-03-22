namespace SharedKernel.MassTransit.Test.Domain;

public sealed class OrderEntity
{
    public Guid Id { get; set; }

    public string CustomerId { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }

    public DateTime UpdatedAtUtc { get; set; }
}
