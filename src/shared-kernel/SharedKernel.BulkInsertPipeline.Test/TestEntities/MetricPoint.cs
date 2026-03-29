namespace SharedKernel.BulkInsertPipeline.Test.TestEntities;

public sealed record MetricPoint(
    string DeviceId,
    DateTimeOffset OccurredAt,
    double Value,
    int? Quality
);
