namespace SharedKernel.Cache.Options;

/// <summary>
/// Represents configuration options for the cache system.
/// </summary>
public class CacheOptions
{
    /// <summary>
    /// Determines whether the caching mechanism is enabled or disabled.
    /// </summary>
    /// <remarks>
    /// If set to <c>true</c>, caching will be functional and actively used for operations
    /// such as retrieving, adding, and removing cache entries. If set to <c>false</c>,
    /// caching operations will be ignored, and the cache storage will not be utilized.
    /// </remarks>
    public bool Enabled { get; set; }

    /// <summary>
    /// Specifies the URL configuration for connecting to the cache system.
    /// Provides the connection string used to establish connectivity with caching servers,
    /// including optional parameters for advanced configurations and authentication credentials.
    /// </summary>
    public string? UrlConfiguration { get; set; }

    /// <summary>
    /// Gets or sets the instance name used as a prefix for cache keys.
    /// </summary>
    /// <remarks>
    /// The <c>InstanceName</c> property allows specifying a unique prefix for cache keys, enabling
    /// segmentation or isolation of cache data across different instances or applications. This
    /// is especially useful in scenarios where multiple applications or modules share the same
    /// distributed cache infrastructure.
    /// </remarks>
    public string? InstanceName { get; set; }

    /// <summary>
    /// Gets or sets the default Redis database index used when no explicit database is supplied.
    /// </summary>
    public int DefaultDatabase { get; set; } = 0;

    /// <summary>
    /// Gets or sets the sliding expiration time, in seconds, for cached items.
    /// Sliding expiration resets the expiration timer each time the cached item is accessed.
    /// </summary>
    public int SlidingExpirationInSecond { get; set; }

    /// <summary>
    /// Gets or sets the default sliding expiration time, in seconds, for in-memory caching.
    /// This property determines the duration of time a cache entry will remain in memory
    /// before being removed when there is no access or update of the entry within the set time frame.
    /// The default value is 3600 seconds (1 hour).
    /// </summary>
    public int MemoryDefaultSlidingExpirationInSecond { get; set; } = 3600;

    /// <summary>
    /// Specifies the default sliding expiration time, in seconds, for Redis cached entries.
    /// This property determines the time span during which a cached entry will remain
    /// accessible from the point of the last access. If the cache is accessed within this
    /// time frame, the sliding expiration time is reset.
    /// </summary>
    public int RedisDefaultSlidingExpirationInSecond { get; set; } = 3600;

    /// <summary>
    /// Specifies the maximum time, in milliseconds, to wait for a connection to be established
    /// with the Redis server before timing out.
    /// A lower value may result in faster failure in case of connectivity issues,
    /// while a higher value may allow for more time during high-latency scenarios.
    /// The default value is 5000 milliseconds.
    /// </summary>
    public int ConnectTimeout { get; set; } = 60000;

    /// <summary>
    /// Represents the timeout duration in milliseconds for synchronous operations.
    /// Applies specifically to Redis-related synchronization tasks, determining how long
    /// the operation waits for a response before timing out.
    /// </summary>
    public int SyncTimeout { get; set; } = 60000;

    /// <summary>
    /// Specifies the timeout duration, in milliseconds, for asynchronous operations.
    /// </summary>
    /// <remarks>
    /// This property defines the maximum time an asynchronous operation can take before it is
    /// considered to have timed out. If the operation exceeds this duration, a timeout exception
    /// is typically thrown. Adjust this value to accommodate network conditions or specific
    /// application requirements.
    /// </remarks>
    public int AsyncTimeout { get; set; } = 60000;

    /// <summary>
    /// Gets or sets the delta backoff time in milliseconds.
    /// This property is commonly used to specify the delay duration for retry mechanisms
    /// when retrying failed operations due to transient issues.
    /// A higher value increases the delay between retries while a lower value decreases it.
    /// </summary>
    public int DeltaBackoffMilliseconds { get; set; } = 10000;

    /// <summary>
    /// Gets or sets the default port number used for connections.
    /// The default value is 6379, which is typically the default port
    /// used for Redis services.
    /// </summary>
    public int DefaultPort { get; set; } = 6379;

    /// <summary>
    /// Gets or sets the size of the Redis connection pool. This determines the number of
    /// concurrent connections available for pooling. A larger pool size allows more
    /// simultaneous requests to be handled but may consume more resources.
    /// </summary>
    public int PoolSize { get; set; } = 25;

    /// <summary>
    /// Gets or sets a value indicating whether SSL (Secure Sockets Layer) is enabled for Redis connections.
    /// </summary>
    /// <remarks>
    /// When set to true, SSL will be used for secure communications between the application and the Redis server.
    /// This ensures that data transmitted over the connection is encrypted. By default, this property is false.
    /// </remarks>
    public bool Ssl { get; set; } = false;
}
