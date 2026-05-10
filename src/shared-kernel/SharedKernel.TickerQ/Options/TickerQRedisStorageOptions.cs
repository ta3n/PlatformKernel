namespace SharedKernel.TickerQ.Options;

/// <summary>
/// Configuration options for TickerQ Redis storage.
/// </summary>
public sealed class TickerQRedisStorageOptions
{
    /// <summary>
    /// Redis connection string. Falls back to ConnectionStrings:TickerQRedisConnection or Redis:UrlConfiguration.
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Redis database number. If not specified, uses Redis:DefaultDatabase from configuration.
    /// </summary>
    public int? Database { get; set; }

    /// <summary>
    /// Key prefix for TickerQ entries in Redis.
    /// </summary>
    public string? Prefix { get; set; } = "tickerq:";

    /// <summary>
    /// Heartbeat interval in seconds for distributed node coordination.
    /// </summary>
    public int? HeartbeatIntervalInSeconds { get; set; }

    /// <summary>
    /// Node timeout in seconds for detecting dead nodes.
    /// </summary>
    public int? NodeTimeoutInSeconds { get; set; }
}
