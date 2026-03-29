namespace SharedKernel.BulkInsertPipeline.Test.Service.Domain;

public sealed class MetricReading
{
    public long Id { get; set; }

    public required string DeviceId { get; set; }

    public DateTimeOffset OccurredAt { get; set; }

    public double Value { get; set; }

    public int? Quality { get; set; }

    public required string Source { get; set; }
}
