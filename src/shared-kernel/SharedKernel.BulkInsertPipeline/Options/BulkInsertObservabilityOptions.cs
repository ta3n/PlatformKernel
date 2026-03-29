namespace SharedKernel.BulkInsertPipeline.Options;

public sealed class BulkInsertObservabilityOptions
{
    public bool EnableMetrics { get; set; } = true;

    public bool EnableTracing { get; set; } = true;

    public bool EnableDetailedSuccessLogs { get; set; }
}
