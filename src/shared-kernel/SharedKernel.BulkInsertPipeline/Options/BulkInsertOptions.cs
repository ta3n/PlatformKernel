namespace SharedKernel.BulkInsertPipeline.Options;

public sealed class BulkInsertOptions
{
    public int WorkerCount { get; set; } = 2;

    public int ChannelCapacity { get; set; } = 100_000;

    public bool EnableBackpressure { get; set; } = true;

    public BulkInsertOverloadStrategy OverloadStrategy { get; set; } = BulkInsertOverloadStrategy.Wait;

    public TimeSpan ShutdownDrainTimeout { get; set; } = TimeSpan.FromSeconds(30);

    public int MaxFallbackCommandParameters { get; set; } = 60_000;

    public BulkInsertProviderType PrimaryProvider { get; private set; } = BulkInsertProviderType.NpgsqlBinaryCopy;

    public AdaptiveBatchOptions AdaptiveBatching { get; } = new();

    public BulkInsertResilienceOptions Resilience { get; } = new();

    public BulkInsertObservabilityOptions Observability { get; } = new();

    public void UseNpgsqlCopy()
    {
        PrimaryProvider = BulkInsertProviderType.NpgsqlBinaryCopy;
    }

    public void UseDapper()
    {
        PrimaryProvider = BulkInsertProviderType.Dapper;
    }

    public void UseRepoDb()
    {
        PrimaryProvider = BulkInsertProviderType.RepoDb;
    }

    public void UseEfCore()
    {
        PrimaryProvider = BulkInsertProviderType.EfCore;
    }

    public void EnableAdaptiveBatching()
    {
        AdaptiveBatching.Enabled = true;
    }

    public void DisableAdaptiveBatching()
    {
        AdaptiveBatching.Enabled = false;
    }

    public void EnableMetrics()
    {
        Observability.EnableMetrics = true;
    }

    public void DisableMetrics()
    {
        Observability.EnableMetrics = false;
    }

    public void EnableTracing()
    {
        Observability.EnableTracing = true;
    }

    public void DisableTracing()
    {
        Observability.EnableTracing = false;
    }
}
