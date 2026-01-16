using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PlatformKernel.Cache.Options;
using StackExchange.Redis;

namespace PlatformKernel.Cache.Utils;

/// <summary>
/// Manages a pool of Redis connections, allowing for efficient reuse and scalability
/// when interacting with a Redis cluster.
/// </summary>
public class RedisConnectionPool
{
    /// <summary>
    /// Provides logging functionality for the <see cref="RedisConnectionPool"/> class.
    /// This variable is used to log information, warnings, errors, and other
    /// messages related to Redis connection management and pool operations.
    /// </summary>
    private readonly ILogger<RedisConnectionPool> _logger;

    /// <summary>
    /// Represents a pool of lazy-initialized Redis connections, enabling efficient management
    /// and reuse of multiple Redis connections within a specified pool size.
    /// </summary>
    /// <remarks>
    /// The pool size is determined by the <see cref="CacheOptions.PoolSize"/> property
    /// and defaults to a minimum of 10 if no explicit value is provided. Each connection
    /// in the pool is represented by a <see cref="Lazy{ConnectionMultiplexer}"/>, allowing
    /// connections to initialize only when accessed. This improves resource utilization
    /// and reduces potential overhead of pre-creating unused connections.
    /// </remarks>
    private readonly Lazy<ConnectionMultiplexer>[] _redisPool;

    /// <summary>
    /// A lazily-initialized single connection to a Redis server, using the `ConnectionMultiplexer` class.
    /// </summary>
    /// <remarks>
    /// This variable is used when the connection pool is configured to use a single Redis connection
    /// across multiple operations. It is initialized and managed with thread-safety guarantees provided
    /// by the `Lazy` class. The configuration for the connection is defined by the `ConfigurationOptions`
    /// object passed during initialization. It helps reduce overhead when a pooled setup is unnecessary
    /// or when the single-connection mode is explicitly enabled.
    /// </remarks>
    private readonly Lazy<ConnectionMultiplexer>? _lazyConnection;

    /// <summary>
    /// Represents the size of the Redis connection pool. Indicates the
    /// number of connections that will be maintained in the pool for
    /// managing requests to the Redis server.
    /// </summary>
    private readonly int _poolSize;

    /// <summary>
    /// Holds the Redis configuration options used to establish and manage connections
    /// to the Redis server. These options are based on the <see cref="CacheOptions"/>
    /// and configured to control the behavior of the Redis connection,
    /// such as timeouts, retry policies, and other connection settings.
    /// </summary>
    private readonly ConfigurationOptions _configOptions;

    /// <summary>
    /// Represents the index used to determine the next connection to be retrieved from the connection pool in a thread-safe manner.
    /// </summary>
    /// <remarks>
    /// This static variable is incremented atomically each time a connection is fetched from the pool,
    /// ensuring that connections are distributed across the pool. The value is periodically reset
    /// to avoid overflow when it surpasses a predefined threshold.
    /// </remarks>
    private static int _nextConnectionIndex;

    /// Indicates whether a single connection to the Redis server should be used across the pool.
    /// When set to true, a single `ConnectionMultiplexer` instance is utilized for all operations,
    /// ensuring simplified management and potentially reduced resource usage. If false, multiple
    /// `ConnectionMultiplexer` instances are created and maintained as a connection pool.
    private static bool IsUseSingleConnection => true;

    /// <summary>
    /// Represents a connection pool for managing Redis connections in a thread-safe manner.
    /// </summary>
    public RedisConnectionPool(
        ILogger<RedisConnectionPool> logger,
        IOptions<CacheOptions> cacheOptions
    )
    {
        _logger = logger;
        var options = cacheOptions.Value;

        _poolSize = options.PoolSize > 0 ? options.PoolSize : 10;
        _redisPool = new Lazy<ConnectionMultiplexer>[_poolSize];
        _configOptions = options.ToExtensionConfigureOptions();

        if (IsUseSingleConnection)
        {
            _lazyConnection = new(
                () => ConnectionMultiplexer.Connect(_configOptions),
                LazyThreadSafetyMode.ExecutionAndPublication
            );
        }
        else
        {
            for (var i = 0; i < _poolSize; i++)
            {
                _redisPool[i] = new Lazy<ConnectionMultiplexer>(
                    () => ConnectionMultiplexer.Connect(_configOptions),
                    LazyThreadSafetyMode.ExecutionAndPublication
                );
            }
        }
    }

    /// <summary>
    /// Retrieves a Redis connection from the connection pool. If pooling is disabled, it returns a single reusable connection.
    /// Otherwise, it selects a connection from the pool, ensuring that unresponsive connections are reconnected as needed.
    /// </summary>
    /// <returns>
    /// A <see cref="StackExchange.Redis.ConnectionMultiplexer"/> instance representing the Redis connection.
    /// </returns>
    public ConnectionMultiplexer GetConnection()
    {
        if (IsUseSingleConnection)
        {
            return _lazyConnection?.Value!;
        }

        var index = Interlocked.Increment(ref _nextConnectionIndex) % _poolSize;
        if (_nextConnectionIndex > _poolSize * 1000)
        {
            Interlocked.Exchange(ref _nextConnectionIndex, 0);
        }

        var connection = _redisPool[index].Value;

        try
        {
            if (connection.IsConnected && connection.GetDatabase().Ping() < TimeSpan.FromMilliseconds(500))
            {
                return connection;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Redis connection at index {Index} is unresponsive. Reconnecting...",
                index
            );
        }

        _logger.LogInformation(
            "Redis connection at index {Index} is down. Reconnecting...",
            index
        );

        Reconnect(index);

        return _redisPool[index].Value;
    }

    /// <summary>
    /// Reconnects to the Redis server at the specified index within the connection pool.
    /// Replaces the current connection with a new Lazy-initialized connection.
    /// </summary>
    /// <param name="index">The index of the Redis connection to be reconnected within the pool.</param>
    private void Reconnect(
        int index
    )
    {
        try
        {
            _redisPool[index].Value.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to dispose Redis connection at index {Index}: {ExMessage}",
                index,
                ex.Message
            );
        }

        _redisPool[index] = new Lazy<ConnectionMultiplexer>(
            () => ConnectionMultiplexer.Connect(_configOptions),
            LazyThreadSafetyMode.ExecutionAndPublication
        );
    }
}
