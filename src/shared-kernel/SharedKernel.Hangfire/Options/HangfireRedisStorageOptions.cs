namespace SharedKernel.Hangfire.Options;

/// <summary>
/// Represents Redis-specific storage options for Hangfire.
/// </summary>
public sealed class HangfireRedisStorageOptions
{
    /// <summary>
    /// Gets or sets the Redis connection string used by Hangfire storage.
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the Redis database index to use for Hangfire storage.
    /// </summary>
    public int? Database { get; set; }

    /// <summary>
    /// Gets or sets the Redis key prefix used by Hangfire storage.
    /// </summary>
    public string? Prefix { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of succeeded jobs kept in the Redis succeeded list.
    /// </summary>
    public int? SucceededListSize { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of deleted jobs kept in the Redis deleted list.
    /// </summary>
    public int? DeletedListSize { get; set; }

    /// <summary>
    /// Gets or sets the interval, in seconds, used to scan for expired Hangfire data.
    /// </summary>
    public int? ExpiryCheckIntervalInSeconds { get; set; }

    /// <summary>
    /// Gets or sets the timeout, in seconds, used when fetching jobs from Redis.
    /// </summary>
    public int? FetchTimeoutInSeconds { get; set; }

    /// <summary>
    /// Gets or sets the invisibility timeout, in seconds, for fetched jobs.
    /// </summary>
    public int? InvisibilityTimeoutInSeconds { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether Redis transactions are used.
    /// </summary>
    public bool? UseTransactions { get; set; }
}
