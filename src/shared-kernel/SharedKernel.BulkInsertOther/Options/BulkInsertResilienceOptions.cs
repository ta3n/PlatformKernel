namespace SharedKernel.BulkInsertOther.Options;

public sealed class BulkInsertResilienceOptions
{
    public bool EnableRetry { get; set; } = true;

    public int MaxRetryAttempts { get; set; } = 3;

    public TimeSpan BaseRetryDelay { get; set; } = TimeSpan.FromMilliseconds(100);

    public bool EnableCircuitBreaker { get; set; } = true;

    public int CircuitBreakerFailureThreshold { get; set; } = 8;

    public TimeSpan CircuitBreakerBreakDuration { get; set; } = TimeSpan.FromSeconds(15);

    public BulkInsertProviderType? FallbackProvider { get; set; } = BulkInsertProviderType.Dapper;
}
