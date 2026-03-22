namespace SharedKernel.MassTransit.Test.Domain;

public sealed class ProcessingLog
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }

    public string Step { get; set; } = string.Empty;

    public string Source { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }
}
