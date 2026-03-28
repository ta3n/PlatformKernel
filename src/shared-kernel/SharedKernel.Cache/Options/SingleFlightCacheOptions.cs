namespace SharedKernel.Cache.Options;

/// <summary>
/// Configures stampede protection behavior for a single-flight cache request.
/// </summary>
public sealed class SingleFlightCacheOptions
{
    /// <summary>
    /// Gets or sets the time-to-live for the primary cache entry.
    /// When null, the cache service default expiration policy is used.
    /// </summary>
    public TimeSpan? FreshTtl { get; set; }

    /// <summary>
    /// Gets or sets the time-to-live for the stale cache entry.
    /// When null, stale responses are disabled.
    /// </summary>
    public TimeSpan? StaleTtl { get; set; }

    /// <summary>
    /// Gets or sets how long competing requests wait for the lock holder to populate the cache.
    /// </summary>
    public TimeSpan WaitTimeout { get; set; } = TimeSpan.FromSeconds(3);

    /// <summary>
    /// Gets or sets how long the distributed lock should live.
    /// </summary>
    public TimeSpan LockTtl { get; set; } = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Gets or sets how often waiting requests re-check the cache.
    /// </summary>
    public TimeSpan PollInterval { get; set; } = TimeSpan.FromMilliseconds(100);

    /// <summary>
    /// Gets or sets whether stale data should be returned after the wait timeout elapses.
    /// </summary>
    public bool ServeStaleOnWaitTimeout { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the request should execute the origin factory when the wait timeout elapses.
    /// </summary>
    public bool AllowOriginFallback { get; set; } = true;

    /// <summary>
    /// Gets or sets whether origin fallback responses should be written back into cache.
    /// </summary>
    public bool CacheOriginFallbackResult { get; set; } = true;

    /// <summary>
    /// Gets or sets an optional explicit lock key. When null, a namespaced key is derived from the cache key.
    /// </summary>
    public string? LockKey { get; set; }

    /// <summary>
    /// Gets or sets an optional explicit stale key. When null, a namespaced key is derived from the cache key.
    /// </summary>
    public string? StaleKey { get; set; }
}
