namespace SharedKernel.TickerQ.Options;

/// <summary>
/// Configuration options for TickerQ storage provider.
/// </summary>
public sealed class TickerQStorageOptions
{
    /// <summary>
    /// Storage provider type: "EntityFramework" or "Redis".
    /// </summary>
    public string Provider { get; set; } = TickerQStorageProviders.EntityFramework;

    /// <summary>
    /// Connection string name for Entity Framework storage.
    /// </summary>
    public string? ConnectionStringName { get; set; } = "TickerQConnection";

    /// <summary>
    /// Redis storage options.
    /// </summary>
    public TickerQRedisStorageOptions Redis { get; set; } = new();

    /// <summary>
    /// Whether to cancel missed tickers on application restart.
    /// </summary>
    public bool CancelMissedTickersOnRestart { get; set; } = true;

    /// <summary>
    /// Whether to use model customizer for EF Core migrations.
    /// </summary>
    public bool UseModelCustomizerForMigrations { get; set; } = true;
}

/// <summary>
/// Defines supported TickerQ storage provider types.
/// </summary>
public static class TickerQStorageProviders
{
    public const string EntityFramework = "EntityFramework";
    public const string Redis = "Redis";
}
