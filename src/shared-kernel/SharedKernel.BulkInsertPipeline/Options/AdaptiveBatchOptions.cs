namespace SharedKernel.BulkInsertPipeline.Options;

public sealed class AdaptiveBatchOptions
{
    public bool Enabled { get; set; } = true;

    public int MinBatchSize { get; set; } = 500;

    public int MaxBatchSize { get; set; } = 5_000;

    public TimeSpan MaxDelay { get; set; } = TimeSpan.FromMilliseconds(100);

    public double IncreaseMultiplier { get; set; } = 1.35d;

    public double DecreaseMultiplier { get; set; } = 0.75d;
}
