using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedKernel.Cache.Models;
using SharedKernel.Cache.Options;
using SharedKernel.Cache.Utils;
using StackExchange.Redis;

namespace SharedKernel.Cache.Services;

/// <summary>
/// Provides Redis-native cache operations with support for per-request database selection.
/// </summary>
public sealed class RedisCacheService(
    ILogger<RedisCacheService> logger,
    IOptions<CacheOptions> cacheOptions,
    RedisConnectionPool redisConnectionPool
) : IRedisCacheService
{
    private readonly CacheOptions _cacheOptions = cacheOptions.Value;

    public IDatabase GetDatabase(
        int? database = null
    )
    {
        return redisConnectionPool.GetDatabase(
            CacheHelper.ResolveDatabaseIndex(
                database,
                _cacheOptions.DefaultDatabase
            )
        );
    }

    public string BuildKey(
        string key,
        bool ignoreInstanceName = false
    )
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return key;
        }

        if (ignoreInstanceName || string.IsNullOrWhiteSpace(_cacheOptions.InstanceName))
        {
            return key;
        }

        return $"{_cacheOptions.InstanceName}{key}";
    }

    public async Task<string?> StringGetAsync(
        string key,
        RedisCacheCommandOptions? options = null,
        CancellationToken cancellationToken = default
    )
    {
        if (!_cacheOptions.Enabled || string.IsNullOrWhiteSpace(key))
        {
            return null;
        }

        try
        {
            var redisValue = await GetDatabase(options?.Database)
                .StringGetAsync(
                    BuildKey(
                        key,
                        options?.IgnoreInstanceName ?? false
                    )
                );

            return redisValue.IsNullOrEmpty ? null : redisValue.ToString();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get Redis string for key {Key}", key);
            return null;
        }
    }

    public async Task<T?> StringGetAsync<T>(
        string key,
        RedisCacheCommandOptions? options = null,
        CancellationToken cancellationToken = default
    )
    {
        var rawValue = await StringGetAsync(
            key,
            options,
            cancellationToken
        );

        return DeserializeValue<T>(rawValue);
    }

    public async Task<IReadOnlyDictionary<string, string?>> StringGetManyAsync(
        IEnumerable<string> keys,
        RedisCacheCommandOptions? options = null,
        CancellationToken cancellationToken = default
    )
    {
        var keyList = keys
            .Where(static key => !string.IsNullOrWhiteSpace(key))
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        if (!_cacheOptions.Enabled || keyList.Length == 0)
        {
            return new Dictionary<string, string?>(StringComparer.Ordinal);
        }

        var redisKeys = keyList
            .Select(
                key => (RedisKey)BuildKey(
                    key,
                    options?.IgnoreInstanceName ?? false
                )
            )
            .ToArray();

        var values = await GetDatabase(options?.Database)
            .StringGetAsync(redisKeys);

        var result = new Dictionary<string, string?>(
            keyList.Length,
            StringComparer.Ordinal
        );

        for (var index = 0; index < keyList.Length; index++)
        {
            result[keyList[index]] = values[index].IsNullOrEmpty
                ? null
                : values[index].ToString();
        }

        return result;
    }

    public async Task<bool> StringSetAsync(
        string key,
        string value,
        RedisCacheCommandOptions? options = null,
        When when = When.Always,
        CancellationToken cancellationToken = default
    )
    {
        if (!_cacheOptions.Enabled || string.IsNullOrWhiteSpace(key))
        {
            return false;
        }

        return await GetDatabase(options?.Database)
            .StringSetAsync(
                BuildKey(
                    key,
                    options?.IgnoreInstanceName ?? false
                ),
                value,
                options?.Expiry,
                when
            );
    }

    public async Task<bool> StringSetAsync<T>(
        string key,
        T value,
        RedisCacheCommandOptions? options = null,
        When when = When.Always,
        CancellationToken cancellationToken = default
    )
    {
        return await StringSetAsync(
            key,
            SerializeValue(value),
            options,
            when,
            cancellationToken
        );
    }

    public async Task<long> StringSetBulkAsync(
        IReadOnlyDictionary<string, string> keyValuePairs,
        RedisCacheCommandOptions? options = null,
        CancellationToken cancellationToken = default
    )
    {
        if (!_cacheOptions.Enabled || keyValuePairs.Count == 0)
        {
            return 0;
        }

        var database = GetDatabase(options?.Database);
        var redisPairs = keyValuePairs
            .Where(static pair => !string.IsNullOrWhiteSpace(pair.Key))
            .Select(
                pair => new KeyValuePair<RedisKey, RedisValue>(
                    BuildKey(
                        pair.Key,
                        options?.IgnoreInstanceName ?? false
                    ),
                    pair.Value
                )
            )
            .ToArray();

        if (redisPairs.Length == 0)
        {
            return 0;
        }

        await database.StringSetAsync(redisPairs);

        if (options?.Expiry is { } expiry)
        {
            var expiryTasks = redisPairs
                .Select(
                    pair => database.KeyExpireAsync(
                        pair.Key,
                        expiry
                    )
                );

            await Task.WhenAll(expiryTasks);
        }

        return redisPairs.Length;
    }

    public async Task<long> StringIncrementAsync(
        string key,
        long value = 1,
        RedisCacheCommandOptions? options = null,
        CancellationToken cancellationToken = default
    )
    {
        if (!_cacheOptions.Enabled || string.IsNullOrWhiteSpace(key))
        {
            return 0;
        }

        return await GetDatabase(options?.Database)
            .StringIncrementAsync(
                BuildKey(
                    key,
                    options?.IgnoreInstanceName ?? false
                ),
                value
            );
    }

    public async Task<bool> HashSetAsync<T>(
        string key,
        string field,
        T value,
        RedisCacheCommandOptions? options = null,
        CancellationToken cancellationToken = default
    )
    {
        if (!_cacheOptions.Enabled || string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(field))
        {
            return false;
        }

        var database = GetDatabase(options?.Database);
        var redisKey = BuildKey(
            key,
            options?.IgnoreInstanceName ?? false
        );

        await database.HashSetAsync(
            redisKey,
            field,
            SerializeValue(value)
        );

        if (options?.Expiry is { } expiry)
        {
            await database.KeyExpireAsync(
                redisKey,
                expiry
            );
        }

        return true;
    }

    public async Task<T?> HashGetAsync<T>(
        string key,
        string field,
        RedisCacheCommandOptions? options = null,
        CancellationToken cancellationToken = default
    )
    {
        if (!_cacheOptions.Enabled || string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(field))
        {
            return default;
        }

        var value = await GetDatabase(options?.Database)
            .HashGetAsync(
                BuildKey(
                    key,
                    options?.IgnoreInstanceName ?? false
                ),
                field
            );

        return value.IsNullOrEmpty
            ? default
            : DeserializeValue<T>(value.ToString());
    }

    public async Task<IReadOnlyDictionary<string, string?>> HashGetAllAsync(
        string key,
        RedisCacheCommandOptions? options = null,
        CancellationToken cancellationToken = default
    )
    {
        if (!_cacheOptions.Enabled || string.IsNullOrWhiteSpace(key))
        {
            return new Dictionary<string, string?>(StringComparer.Ordinal);
        }

        var entries = await GetDatabase(options?.Database)
            .HashGetAllAsync(
                BuildKey(
                    key,
                    options?.IgnoreInstanceName ?? false
                )
            );

        return entries.ToDictionary(
            entry => entry.Name.ToString(),
            entry => entry.Value.IsNullOrEmpty ? null : entry.Value.ToString(),
            StringComparer.Ordinal
        );
    }

    public async Task<bool> KeyDeleteAsync(
        string key,
        RedisCacheCommandOptions? options = null,
        CancellationToken cancellationToken = default
    )
    {
        if (!_cacheOptions.Enabled || string.IsNullOrWhiteSpace(key))
        {
            return false;
        }

        return await GetDatabase(options?.Database)
            .KeyDeleteAsync(
                BuildKey(
                    key,
                    options?.IgnoreInstanceName ?? false
                )
            );
    }

    public async Task<bool> KeyExpireAsync(
        string key,
        TimeSpan expiry,
        RedisCacheCommandOptions? options = null,
        CancellationToken cancellationToken = default
    )
    {
        if (!_cacheOptions.Enabled || string.IsNullOrWhiteSpace(key))
        {
            return false;
        }

        return await GetDatabase(options?.Database)
            .KeyExpireAsync(
                BuildKey(
                    key,
                    options?.IgnoreInstanceName ?? false
                ),
                expiry
            );
    }

    public async Task<IReadOnlyCollection<string>> GetKeysAsync(
        string pattern,
        RedisCacheCommandOptions? options = null,
        int pageSize = 200,
        int maxKeys = 5_000,
        CancellationToken cancellationToken = default
    )
    {
        if (!_cacheOptions.Enabled || string.IsNullOrWhiteSpace(pattern))
        {
            return [];
        }

        var keys = new List<string>();
        var database = CacheHelper.ResolveDatabaseIndex(
            options?.Database,
            _cacheOptions.DefaultDatabase
        );
        var prefixedPattern = BuildKey(
            pattern,
            options?.IgnoreInstanceName ?? false
        );
        var muxer = (IConnectionMultiplexer)redisConnectionPool.GetConnection();

        await foreach (var redisKey in RedisExtension.ScanKeysAsync(
                muxer,
                prefixedPattern,
                database,
                pageSize,
                maxKeys,
                cancellationToken: cancellationToken
            ))
        {
            var value = redisKey.ToString();
            keys.Add(
                StripInstanceName(
                    value,
                    options?.IgnoreInstanceName ?? false
                )
            );
        }

        return keys;
    }

    public async Task<long> RemoveByPatternAsync(
        string pattern,
        RedisCacheCommandOptions? options = null,
        int pageSize = 500,
        int deleteBatchSize = 200,
        int maxKeys = 10_000,
        CancellationToken cancellationToken = default
    )
    {
        if (!_cacheOptions.Enabled || string.IsNullOrWhiteSpace(pattern))
        {
            return 0;
        }

        var database = CacheHelper.ResolveDatabaseIndex(
            options?.Database,
            _cacheOptions.DefaultDatabase
        );
        var prefixedPattern = BuildKey(
            pattern,
            options?.IgnoreInstanceName ?? false
        );
        var muxer = (IConnectionMultiplexer)redisConnectionPool.GetConnection();

        return await RedisExtension.ClearByPatternAsync(
            muxer,
            prefixedPattern,
            database,
            pageSize,
            deleteBatchSize,
            maxKeys,
            cancellationToken
        );
    }

    public async Task<bool> AcquireLockAsync(
        string lockKey,
        string lockValue,
        TimeSpan expiration,
        RedisCacheCommandOptions? options = null
    )
    {
        if (!_cacheOptions.Enabled || string.IsNullOrWhiteSpace(lockKey))
        {
            return false;
        }

        return await GetDatabase(options?.Database)
            .StringSetAsync(
                BuildKey(
                    lockKey,
                    options?.IgnoreInstanceName ?? false
                ),
                lockValue,
                expiration,
                When.NotExists
            );
    }

    public async Task<bool> ReleaseLockAsync(
        string lockKey,
        string lockValue,
        RedisCacheCommandOptions? options = null
    )
    {
        if (!_cacheOptions.Enabled || string.IsNullOrWhiteSpace(lockKey))
        {
            return false;
        }

        const string script = @"
            if redis.call('get', KEYS[1]) == ARGV[1] then
                return redis.call('del', KEYS[1])
            else
                return 0
            end";

        var result = (int)await GetDatabase(options?.Database)
            .ScriptEvaluateAsync(
                script,
                [
                    BuildKey(
                        lockKey,
                        options?.IgnoreInstanceName ?? false
                    )
                ],
                [
                    lockValue
                ]
            );

        return result == 1;
    }

    public async Task<T?> ExecuteLuaByNameWithResultAsync<T>(
        string scriptName,
        string[]? keys = null,
        RedisValue[]? values = null,
        RedisCacheCommandOptions? options = null
    )
    {
        if (!_cacheOptions.Enabled)
        {
            return default;
        }

        var redisKeys = keys?
                .Where(static key => !string.IsNullOrWhiteSpace(key))
                .Select(
                    key => (RedisKey)BuildKey(
                        key,
                        options?.IgnoreInstanceName ?? false
                    )
                )
                .ToArray()
            ?? [];

        var script = LuaScriptLoader.Load(scriptName);
        var result = await GetDatabase(options?.Database)
            .ScriptEvaluateAsync(
                script,
                redisKeys,
                values
            );

        if (result.IsNull)
        {
            return default;
        }

        return DeserializeValue<T>(result.ToString());
    }

    private string StripInstanceName(
        string value,
        bool ignoreInstanceName
    )
    {
        if (ignoreInstanceName || string.IsNullOrWhiteSpace(_cacheOptions.InstanceName))
        {
            return value;
        }

        return value.StartsWith(
            _cacheOptions.InstanceName,
            StringComparison.Ordinal
        )
            ? value[_cacheOptions.InstanceName.Length..]
            : value;
    }

    private static string SerializeValue<T>(
        T value
    )
    {
        return value switch
        {
            null => string.Empty,
            string text => text,
            _ => CacheHelper.Serialize(value) ?? string.Empty
        };
    }

    private static T? DeserializeValue<T>(
        string? rawValue
    )
    {
        if (string.IsNullOrWhiteSpace(rawValue))
        {
            return default;
        }

        if (typeof(T) == typeof(string))
        {
            return (T)(object)rawValue;
        }

        return CacheHelper.Deserialize<T>(rawValue);
    }
}
