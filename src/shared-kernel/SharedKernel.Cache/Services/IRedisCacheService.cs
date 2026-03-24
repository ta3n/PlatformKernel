using SharedKernel.Cache.Models;
using StackExchange.Redis;

namespace SharedKernel.Cache.Services;

/// <summary>
/// Provides Redis-specialized operations that expose database selection, raw Redis data structures, and Lua execution.
/// </summary>
public interface IRedisCacheService
{
    /// <summary>
    /// Gets a Redis database using the configured default database when no index is supplied.
    /// </summary>
    IDatabase GetDatabase(
        int? database = null
    );

    /// <summary>
    /// Builds a Redis key, applying the configured instance prefix unless ignored.
    /// </summary>
    string BuildKey(
        string key,
        bool ignoreInstanceName = false
    );

    Task<string?> StringGetAsync(
        string key,
        RedisCacheCommandOptions? options = null,
        CancellationToken cancellationToken = default
    );

    Task<T?> StringGetAsync<T>(
        string key,
        RedisCacheCommandOptions? options = null,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyDictionary<string, string?>> StringGetManyAsync(
        IEnumerable<string> keys,
        RedisCacheCommandOptions? options = null,
        CancellationToken cancellationToken = default
    );

    Task<bool> StringSetAsync(
        string key,
        string value,
        RedisCacheCommandOptions? options = null,
        When when = When.Always,
        CancellationToken cancellationToken = default
    );

    Task<bool> StringSetAsync<T>(
        string key,
        T value,
        RedisCacheCommandOptions? options = null,
        When when = When.Always,
        CancellationToken cancellationToken = default
    );

    Task<long> StringSetBulkAsync(
        IReadOnlyDictionary<string, string> keyValuePairs,
        RedisCacheCommandOptions? options = null,
        CancellationToken cancellationToken = default
    );

    Task<long> StringIncrementAsync(
        string key,
        long value = 1,
        RedisCacheCommandOptions? options = null,
        CancellationToken cancellationToken = default
    );

    Task<bool> HashSetAsync<T>(
        string key,
        string field,
        T value,
        RedisCacheCommandOptions? options = null,
        CancellationToken cancellationToken = default
    );

    Task<T?> HashGetAsync<T>(
        string key,
        string field,
        RedisCacheCommandOptions? options = null,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyDictionary<string, string?>> HashGetAllAsync(
        string key,
        RedisCacheCommandOptions? options = null,
        CancellationToken cancellationToken = default
    );

    Task<bool> KeyDeleteAsync(
        string key,
        RedisCacheCommandOptions? options = null,
        CancellationToken cancellationToken = default
    );

    Task<bool> KeyExpireAsync(
        string key,
        TimeSpan expiry,
        RedisCacheCommandOptions? options = null,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyCollection<string>> GetKeysAsync(
        string pattern,
        RedisCacheCommandOptions? options = null,
        int pageSize = 200,
        int maxKeys = 5_000,
        CancellationToken cancellationToken = default
    );

    Task<long> RemoveByPatternAsync(
        string pattern,
        RedisCacheCommandOptions? options = null,
        int pageSize = 500,
        int deleteBatchSize = 200,
        int maxKeys = 10_000,
        CancellationToken cancellationToken = default
    );

    Task<bool> AcquireLockAsync(
        string lockKey,
        string lockValue,
        TimeSpan expiration,
        RedisCacheCommandOptions? options = null
    );

    Task<bool> ReleaseLockAsync(
        string lockKey,
        string lockValue,
        RedisCacheCommandOptions? options = null
    );

    Task<T?> ExecuteLuaByNameWithResultAsync<T>(
        string scriptName,
        string[]? keys = null,
        RedisValue[]? values = null,
        RedisCacheCommandOptions? options = null
    );
}
