using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace SharedKernel.Cache.Utils;

/// <summary>
/// Provides a Redis-based implementation of the <see cref="IDistributedCache"/> interface,
/// enabling distributed caching functionality for applications using Redis as the backing store.
/// </summary>
public class RedisDistributedCache(
    RedisConnectionPool redisConnectionPool
) : IDistributedCache
{
    /// <summary>
    /// Represents the default expiration time, in seconds, for cached items.
    /// If no explicit expiration is provided, this value is used to determine
    /// the time-to-live for cached data.
    /// </summary>
    private readonly int _defaultExpirationSeconds = 300;

    /// <summary>
    /// Retrieves a Redis database instance from the connection pool.
    /// This is used to perform various Redis operations, such as storing and retrieving data.
    /// </summary>
    /// <returns>
    /// An <see cref="StackExchange.Redis.IDatabase"/> instance representing the Redis database connection.
    /// </returns>
    private IDatabase GetDatabase()
    {
        return redisConnectionPool.GetDatabase();
    }

    /// <summary>
    /// Retrieves the value from the Redis cache associated with the specified key.
    /// </summary>
    /// <param name="key">The key identifying the value to retrieve from the cache.</param>
    /// <returns>
    /// A byte array containing the cached value if the key exists; otherwise, null.
    /// </returns>
    public byte[]? Get(
        string key
    )
    {
        var value = GetDatabase().StringGet(key);
        return value.IsNullOrEmpty ? null : (byte[])value!;
    }

    /// <summary>
    /// Retrieves an item from the Redis cache asynchronously.
    /// </summary>
    /// <param name="key">The key of the cache entry to retrieve.</param>
    /// <param name="token">A cancellation token to observe while waiting for the task to complete. Optional.</param>
    /// <returns>A byte array representing the cached value, or null if the key does not exist.</returns>
    public async Task<byte[]?> GetAsync(
        string key,
        CancellationToken token = default
    )
    {
        var value = await GetDatabase().StringGetAsync(key);
        return value.IsNullOrEmpty ? null : (byte[])value!;
    }

    /// <summary>
    /// Sets a value in the cache with the specified key and expiration options.
    /// </summary>
    /// <param name="key">The key to store the value under.</param>
    /// <param name="value">The byte array representing the value to be cached.</param>
    /// <param name="options">
    /// Specifies the cache entry options which include parameters like
    /// expiration time or sliding expiration.
    /// </param>
    public void Set(
        string key,
        byte[] value,
        DistributedCacheEntryOptions options
    )
    {
        var expiry = options.AbsoluteExpirationRelativeToNow ?? TimeSpan.FromSeconds(_defaultExpirationSeconds);
        GetDatabase().StringSet(key, value, expiry);
    }

    /// <summary>
    /// Asynchronously stores a value in the Redis cache with the specified key and expiration options.
    /// </summary>
    /// <param name="key">The key under which the value should be stored in the cache.</param>
    /// <param name="value">The value to store in the cache as a byte array.</param>
    /// <param name="options">The cache entry options that specify expiration and other settings.</param>
    /// <param name="token">An optional cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task SetAsync(
        string key,
        byte[] value,
        DistributedCacheEntryOptions options,
        CancellationToken token = default
    )
    {
        var expiry = options.AbsoluteExpirationRelativeToNow ?? TimeSpan.FromSeconds(_defaultExpirationSeconds);
        await GetDatabase().StringSetAsync(key, value, expiry);
    }

    /// <summary>
    /// Refreshes the expiration time of the cache entry for the specified key.
    /// If the key exists in the cache, the expiration time is reset to the default expiration time.
    /// </summary>
    /// <param name="key">The key of the cache entry to refresh.</param>
    public void Refresh(
        string key
    )
    {
        var db = GetDatabase();
        var value = db.StringGet(key);
        if (!value.IsNullOrEmpty)
        {
            db.StringSet(key, value, TimeSpan.FromSeconds(_defaultExpirationSeconds));
        }
    }

    /// <summary>
    /// Refreshes an item in the Redis cache asynchronously by resetting its expiration time.
    /// If the specified key exists in the Redis cache, this method retrieves the value associated
    /// with the key and updates its expiration time to the default expiration period.
    /// </summary>
    /// <param name="key">The cache key of the item to refresh.</param>
    /// <param name="token">An optional cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous refresh operation.</returns>
    public async Task RefreshAsync(
        string key,
        CancellationToken token = default
    )
    {
        var db = GetDatabase();
        var value = await db.StringGetAsync(key);
        if (!value.IsNullOrEmpty)
        {
            await db.StringSetAsync(key, value, TimeSpan.FromSeconds(_defaultExpirationSeconds));
        }
    }

    /// <summary>
    /// Removes the specified key and its associated value from the Redis cache.
    /// </summary>
    /// <param name="key">The cache key to be removed.</param>
    public void Remove(
        string key
    )
    {
        GetDatabase().KeyDelete(key);
    }

    /// <summary>
    /// Asynchronously removes the specified cache entry from Redis.
    /// </summary>
    /// <param name="key">The key of the cache entry to remove.</param>
    /// <param name="token">A CancellationToken to signal the operation should be canceled.</param>
    /// <returns>A Task that represents the asynchronous operation.</returns>
    public async Task RemoveAsync(
        string key,
        CancellationToken token = default
    )
    {
        await GetDatabase().KeyDeleteAsync(key);
    }
}
