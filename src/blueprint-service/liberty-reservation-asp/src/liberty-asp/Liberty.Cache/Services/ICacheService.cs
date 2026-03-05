using Liberty.Cache.Options;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace Liberty.Cache.Services;

/// <summary>
/// Provides an interface for caching services that support basic cache manipulation,
/// advanced key management, asynchronous operations, and distributed cache configurations.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Gets a value indicating whether the caching service is enabled.
    /// </summary>
    /// <value>
    /// Returns <c>true</c> if caching is enabled; otherwise, <c>false</c>.
    /// This value is determined by the <see cref="CacheOptions.Enabled"/> property.
    /// </value>
    bool IsEnabled { get; }

    /// <summary>
    /// Retrieves an interface to interact with a Redis database.
    /// </summary>
    /// <returns>
    /// An instance of <see cref="IDatabase"/> representing the Redis database interface.
    /// </returns>
    IDatabase GetDatabase();

    /// <summary>
    /// Configures the cache settings for the service using the provided <see cref="CacheOptions"/>.
    /// </summary>
    /// <param name="cacheOptions">
    /// An instance of <see cref="CacheOptions"/> containing the configuration details for the cache.
    /// </param>
    void SetOptionSetting(
        CacheOptions cacheOptions
    );

    /// Retrieves a cached item based on the specified key.
    /// The value retrieved is deserialized to the specified type parameter.
    /// Returns the default value of the type if the key does not exist or the value is not found.
    /// <param name="key">The key corresponding to the cached item to be retrieved.</param>
    /// <typeparam name="T">The type of the object to be retrieved from the cache.</typeparam>
    /// <returns>The deserialized object of type T if the key exists and the value is valid, otherwise the default value of type T.</returns>
    T? Get<T>(
        string key
    );

    /// <summary>
    /// Stores a value in the cache.
    /// </summary>
    /// <typeparam name="T">The type of the value to be stored.</typeparam>
    /// <param name="key">The unique key used to identify the cached value.</param>
    /// <param name="value">The value to be stored in the cache.</param>
    void Set<T>(
        string key,
        T value
    );

    /// <summary>
    /// Sets the specified value in the cache with the provided key
    /// and configuration for cache entry options.
    /// </summary>
    /// <typeparam name="T">The type of the object to be stored in the cache.</typeparam>
    /// <param name="key">The key with which the value should be associated in the cache.</param>
    /// <param name="value">The value to store in the cache.</param>
    /// <param name="distributedCacheEntryOptions">
    /// Specifies options for the cache entry, such as expiration policies.
    /// </param>
    void Set<T>(
        string key,
        T value,
        DistributedCacheEntryOptions distributedCacheEntryOptions
    );

    /// <summary>
    /// Removes the cache entry associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the cache entry to remove.</param>
    void Remove(
        string key
    );

    /// <summary>
    /// Removes cached items that match the specified key pattern.
    /// </summary>
    /// <param name="pattern">The key pattern used to identify the cache entries to be removed.</param>
    /// <returns>Returns true if the operation was successful; otherwise, false.</returns>
    bool RemoveByPattern(
        string pattern
    );

    /// Removes cache entries based on specified patterns.
    /// This method enables clearing multiple cache entries by pattern matching.
    /// <param name="ignoreInstance">
    /// A boolean value indicating whether to ignore instance-specific cache entries.
    /// If set to true, instance-specific keys will not be part of the removal process.
    /// </param>
    /// <param name="patterns">
    /// One or more string patterns used to identify cache entries to remove.
    /// The patterns support wildcards for flexible matching of keys.
    /// </param>
    void RemoveByPatterns(
        bool ignoreInstance,
        params string[] patterns
    );

    /// <summary>
    /// Removes cached entries that match the specified patterns.
    /// </summary>
    /// <param name="ignoreInstance">
    /// A boolean value indicating whether to ignore cache entries that are specific to the calling instance.
    /// If true, instance-specific entries will be ignored during removal.
    /// </param>
    /// <param name="patterns">
    /// A list of string patterns used to identify cache entries for removal.
    /// Each pattern may include wildcard characters for matching multiple entries.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous cache removal operation.
    /// </returns>
    Task RemoveByPatternsAsync(
        bool ignoreInstance,
        params string[] patterns
    );

    /// <summary>
    /// Resets the cache by removing entries that match the specified pattern.
    /// This operation allows for targeted cache clearing based on the provided pattern.
    /// </summary>
    /// <param name="pattern">
    /// The pattern used to match cache entries for removal.
    /// If null, no specific pattern is considered, and the method might reset all entries depending on the implementation.
    /// </param>
    void Reset(
        string? pattern
    );

    /// <summary>
    /// Asynchronously retrieves a cached item associated with the specified key.
    /// </summary>
    /// <typeparam name="T">The type of the cached item to be retrieved.</typeparam>
    /// <param name="key">The cache key of the item to retrieve.</param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests. Default is <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the cached item of type <typeparamref name="T"/>, or <c>null</c> if not found.
    /// </returns>
    Task<T?> GetAsync<T>(
        string key,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves a cached string value based on the specified key.
    /// </summary>
    /// <param name="key">The key used to identify the cached value.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the cached string value
    /// if found, or null if the key does not exist in the cache.
    /// </returns>
    Task<string?> GetStringAsync(
        string key,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Asynchronously sets a specified value in the cache associated with the given key.
    /// </summary>
    /// <typeparam name="T">The type of the value to be cached.</typeparam>
    /// <param name="key">The unique key identifying the cache entry.</param>
    /// <param name="value">The value to be stored in the cache.</param>
    /// <param name="cancellationToken">An optional cancellation token to observe during the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SetAsync<T>(
        string key,
        T value,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Asynchronously sets a value in the cache with the specified key and cache entry options.
    /// </summary>
    /// <typeparam name="T">The type of the value to store in the cache.</typeparam>
    /// <param name="key">The cache key associated with the value.</param>
    /// <param name="value">The value to store in the cache.</param>
    /// <param name="distributedCacheEntryOptions">The options used to configure the cache entry's expiration and other settings.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SetAsync<T>(
        string key,
        T value,
        DistributedCacheEntryOptions distributedCacheEntryOptions,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Asynchronously sets multiple key-value pairs in the cache.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the values to be stored in the cache.
    /// </typeparam>
    /// <param name="keyValuePairs">
    /// A dictionary containing the key-value pairs to be stored in the cache.
    /// The keys are unique identifiers, and the values are the corresponding objects to cache.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests during the asynchronous operation.
    /// Defaults to <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    Task SetBulkAsync<T>(
        Dictionary<string, T> keyValuePairs,
        CancellationToken cancellationToken = default
    );

    Task SetBulkAsync<T>(
        Dictionary<string, T> keyValuePairs,
        int? ttlcustom,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Removes a cached item asynchronously based on the specified key.
    /// </summary>
    /// <param name="key">
    /// The key of the cache item to remove.
    /// </param>
    /// <param name="cancellationToken">
    /// Optional cancellation token to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    Task RemoveAsync(
        string key,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Refreshes the expiration of the asynchronous cache entry identified by the specified key.
    /// </summary>
    /// <param name="key">The cache key identifying the entry to refresh.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task RefreshAsync(
        string key,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves a collection of keys from the cache that match the given pattern asynchronously.
    /// </summary>
    /// <param name="pattern">The pattern to match against cache keys.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation, with a collection of keys as the result.</returns>
    Task<IEnumerable<string>> GetKeysAsync(
        string pattern,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Resets cached data either fully or partially based on the specified pattern and instance settings.
    /// </summary>
    /// <param name="pattern">
    /// Optional pattern to filter specific cache entries that need to be reset. If null, the method will reset all entries.
    /// </param>
    /// <param name="isInstance">
    /// Indicates whether the reset operation should be scoped to the current instance. Defaults to true.
    /// </param>
    /// <param name="cancellationToken">
    /// Token to monitor for cancellation requests during the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// </returns>
    Task ResetAsync(
        string? pattern = null,
        bool isInstance = true,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Attempts to acquire a distributed lock with the specified key, value, and expiration duration.
    /// </summary>
    /// <param name="lockKey">The unique key used to identify the lock.</param>
    /// <param name="lockValue">The value associated with the lock to ensure ownership.</param>
    /// <param name="expiration">The duration for which the lock is valid before it automatically expires.</param>
    /// <returns>Returns true if the lock was successfully acquired; otherwise, false.</returns>
    Task<bool> AcquireLockAsync(
        string lockKey,
        string lockValue,
        TimeSpan expiration
    );

    /// <summary>
    /// Releases a lock associated with the specified key and value.
    /// </summary>
    /// <param name="lockKey">The key associated with the lock to be released.</param>
    /// <param name="lockValue">The value associated with the lock to validate ownership before releasing it.</param>
    /// <returns>
    /// Returns <c>true</c> if the lock is successfully released; otherwise, <c>false</c>.
    /// </returns>
    Task<bool> ReleaseLockAsync(
        string lockKey,
        string lockValue
    );

    /// <summary>
    /// Retrieves a collection of cache keys that match the specified pattern.
    /// </summary>
    /// <param name="pattern">
    /// The pattern to match against cache keys. Supports wildcards for flexible matching.
    /// </param>
    /// <returns>
    /// An enumerable collection of cache keys that match the specified pattern.
    /// </returns>
    IEnumerable<string?> GetByPattern(
        string? pattern
    );

    /// <summary>
    /// Retrieves a collection of cached items that match the specified patterns.
    /// </summary>
    /// <typeparam name="TDto">
    /// The type of the objects to be retrieved from the cache.
    /// </typeparam>
    /// <param name="patterns">
    /// An array of patterns used to match cache keys. Supports wildcards for flexible matching.
    /// </param>
    /// <returns>
    /// An enumerable collection of objects of type <typeparamref name="TDto"/> that match the specified patterns.
    /// </returns>
    IEnumerable<TDto> GetByPatterns<TDto>(
        string[] patterns
    );

    /// <summary>
    /// Asynchronously sets a value in the cache with the specified key, optional time-to-live (TTL),
    /// and an option to ignore instance-specific cache entries.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the value to be stored in the cache.
    /// </typeparam>
    /// <param name="key">
    /// The unique key used to identify the cached value. Can be null if the key is dynamically generated.
    /// </param>
    /// <param name="value">
    /// The value to be stored in the cache.
    /// </param>
    /// <param name="ttl">
    /// The optional time-to-live (TTL) for the cache entry, in seconds. If null, the default TTL is used.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests during the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    Task SetIgnoreInstanceAsync<T>(
        string? key,
        T value,
        int? ttl = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Executes a Lua script stored in the system by its script name and attempts to deserialize
    /// the result into the specified type.
    /// This method is useful when the Lua script returns structured data that needs to be mapped
    /// directly to a strongly typed object.
    /// </summary>
    /// <typeparam name="T">
    /// The expected return type after deserializing the Lua script execution result.
    /// </typeparam>
    /// <param name="scriptName">
    /// The name of the Lua script file to be executed.
    /// </param>
    /// <param name="keys">
    /// An optional array of Redis keys to be passed to the Lua script as KEYS arguments.
    /// </param>
    /// <param name="values">
    /// An optional array of Redis values to be passed to the Lua script as ARGV arguments.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the deserialized
    /// value of type <typeparamref name="T"/>, or null if the result cannot be converted.
    /// </returns>
    Task<T?> ExecuteLuaByNameWithResultAsync<T>(
        string scriptName,
        string[]? keys = null,
        RedisValue[]? values = null
    );
}
