using Liberty.Cache.Options;
using Liberty.Cache.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Liberty.Cache.Services;

public class CacheService(
    ILogger<CacheService> logger,
    IOptions<CacheOptions> redisOptions,
    IDistributedCache distributedCache,
    RedisConnectionPool redisConnectionPool
) : ICacheService
{
    private readonly string _clearCacheLuaScript = LuaScriptLoader.Load("ClearCacheWithScan");

    private readonly string _getKeysLuaScript = LuaScriptLoader.Load("GetKeysWithScan");

    private readonly string _getPatternLuaScript = LuaScriptLoader.Load("GetHashFieldDataByPatternsWithScan");

    private CacheOptions _redisOptions = redisOptions.Value;

    public bool IsEnabled => _redisOptions.Enabled;

    public IDatabase GetDatabase()
    {
        return redisConnectionPool.GetConnection().GetDatabase();
    }

    public void SetOptionSetting(
        CacheOptions cacheOptions
    )
    {
        _redisOptions = cacheOptions;
    }

    public T? Get<T>(
        string key
    )
    {
        if (!_redisOptions.Enabled)
        {
            return default;
        }

        var value = distributedCache.GetString(key);

        return !string.IsNullOrWhiteSpace(value) ? CacheHelper.Deserialize<T>(value) : default;
    }

    public bool RemoveByPattern(
        string pattern
    )
    {
        if (!_redisOptions.Enabled)
        {
            return false;
        }

        var server = redisConnectionPool.GetConnection().GetServers().LastOrDefault();
        var keys = server?.Keys(
            pattern: new RedisValue(
                $"{_redisOptions.InstanceName}{pattern}"
            )
        );
        var result = true;
        if (keys == null)
        {
            return result;
        }

        result = keys.All(key => GetDatabase().KeyDelete(key));

        return result;
    }

    public void RemoveByPatterns(
        bool ignoreInstance,
        params string[] patterns
    )
    {
        if (!_redisOptions.Enabled)
        {
            return;
        }

        if (ignoreInstance)
        {
            GetDatabase()
                .ScriptEvaluate(
                    _clearCacheLuaScript,
                    values:
                    [
                        ..patterns.Select(x => $"{x}")
                    ]
                );
        }
        else
        {
            GetDatabase()
                .ScriptEvaluate(
                    _clearCacheLuaScript,
                    values:
                    [
                        ..patterns.Select(x => $"{_redisOptions.InstanceName}{x}")
                    ]
                );
        }
    }

    public async Task RemoveByPatternsAsync(
        bool ignoreInstance,
        params string[] patterns
    )
    {
        if (!_redisOptions.Enabled)
        {
            return;
        }

        if (ignoreInstance)
        {
            await GetDatabase()
                .ScriptEvaluateAsync(
                    _clearCacheLuaScript,
                    values:
                    [
                        ..patterns.Select(x => $"{x}")
                    ]
                );
        }
        else
        {
            await GetDatabase()
                .ScriptEvaluateAsync(
                    _clearCacheLuaScript,
                    values:
                    [
                        ..patterns.Select(x => $"{_redisOptions.InstanceName}{x}")
                    ]
                );
        }
    }

    public void Reset(
        string? pattern
    )
    {
        if (!_redisOptions.Enabled)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(pattern))
        {
            GetDatabase()
                .ScriptEvaluate(
                    _clearCacheLuaScript,
                    values:
                    [
                        _redisOptions.InstanceName + "*"
                    ]
                );
        }
        else
        {
            GetDatabase()
                .ScriptEvaluate(
                    _clearCacheLuaScript,
                    values:
                    [
                        _redisOptions.InstanceName + pattern
                    ]
                );
        }
    }

    public async Task<T?> GetAsync<T>(
        string key,
        CancellationToken cancellationToken = default
    )
    {
        if (!_redisOptions.Enabled)
        {
            return default;
        }

        var value = await distributedCache.GetStringAsync(key, cancellationToken);

        return !string.IsNullOrWhiteSpace(value) ? CacheHelper.Deserialize<T>(value) : default;
    }

    public async Task<string?> GetStringAsync(
        string key,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            if (!_redisOptions.Enabled)
            {
                return null;
            }

            var value = await distributedCache.GetStringAsync(key, cancellationToken);

            return value;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetStringAsync");
            return null;
        }
    }

    public void Set<T>(
        string key,
        T value
    )
    {
        if (!_redisOptions.Enabled)
        {
            return;
        }

        distributedCache.SetString(key, CacheHelper.Serialize(value) ?? string.Empty);
    }

    public void Set<T>(
        string key,
        T value,
        DistributedCacheEntryOptions distributedCacheEntryOptions
    )
    {
        if (!_redisOptions.Enabled)
        {
            return;
        }

        distributedCache.SetString(
            key,
            CacheHelper.Serialize(value) ?? string.Empty,
            distributedCacheEntryOptions
        );
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        CancellationToken cancellationToken = default
    )
    {
        if (!_redisOptions.Enabled)
        {
            return;
        }

        var cacheValue = CacheHelper.Serialize(value) ?? string.Empty;
        await distributedCache.SetStringAsync(
            key,
            cacheValue,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(
                    _redisOptions.RedisDefaultSlidingExpirationInSecond + Random.Shared.Next(0, 50)
                )
            },
            cancellationToken
        );
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        DistributedCacheEntryOptions distributedCacheEntryOptions,
        CancellationToken cancellationToken = default
    )
    {
        if (!_redisOptions.Enabled)
        {
            return;
        }

        var cacheValue = CacheHelper.Serialize(value) ?? string.Empty;
        await distributedCache.SetStringAsync(
            key,
            cacheValue,
            distributedCacheEntryOptions,
            cancellationToken
        );
    }

    public async Task SetBulkAsync<T>(
        Dictionary<string, T> keyValuePairs,
        CancellationToken cancellationToken = default
    )
    {
        if (!_redisOptions.Enabled || keyValuePairs.Count == 0)
        {
            return;
        }

        var bulkSetScript = LuaScriptLoader.Load("BulkSetScript");

        var database = GetDatabase();
        var ttl = _redisOptions.RedisDefaultSlidingExpirationInSecond + Random.Shared.Next(0, 50);

        var values = new List<RedisValue> { ttl };

        foreach (var kvp in keyValuePairs)
        {
            values.Add($"{_redisOptions.InstanceName}{kvp.Key}");
            values.Add(CacheHelper.Serialize(kvp.Value) ?? string.Empty);
        }

        await database.ScriptEvaluateAsync(bulkSetScript, values: [.. values]);
    }

    public async Task SetBulkAsync<T>(
        Dictionary<string, T> keyValuePairs,
        int? ttlcustom,
        CancellationToken cancellationToken = default
    )
    {
        if (!_redisOptions.Enabled || keyValuePairs.Count == 0)
        {
            return;
        }

        var bulkSetScript = LuaScriptLoader.Load("BulkSetScript");

        var database = GetDatabase();
        var ttl = ttlcustom ?? _redisOptions.RedisDefaultSlidingExpirationInSecond + Random.Shared.Next(0, 50);

        var values = new List<RedisValue> { ttl };

        foreach (var kvp in keyValuePairs)
        {
            values.Add($"{_redisOptions.InstanceName}{kvp.Key}");
            values.Add(CacheHelper.Serialize(kvp.Value) ?? string.Empty);
        }

        await database.ScriptEvaluateAsync(bulkSetScript, values: [.. values]);
    }

    public void Remove(
        string key
    )
    {
        if (!_redisOptions.Enabled)
        {
            return;
        }

        distributedCache.Remove(key);
    }

    public async Task RemoveAsync(
        string key,
        CancellationToken cancellationToken = default
    )
    {
        if (!_redisOptions.Enabled)
        {
            return;
        }

        await distributedCache.RemoveAsync(key, cancellationToken);
    }

    public async Task RefreshAsync(
        string key,
        CancellationToken cancellationToken = default
    )
    {
        if (!_redisOptions.Enabled)
        {
            return;
        }

        await distributedCache.RefreshAsync(key, cancellationToken);
    }

    public async Task<IEnumerable<string>> GetKeysAsync(
        string pattern,
        CancellationToken cancellationToken = default
    )
    {
        if (!_redisOptions.Enabled)
        {
            return [];
        }

        var result = await GetDatabase()
            .ScriptEvaluateAsync(
                _getKeysLuaScript,
                values:
                [
                    _redisOptions.InstanceName + pattern
                ]
            );

        return
        [
            .. ((RedisResult[])result)!.Select(
                x => x.ToString()![_redisOptions.InstanceName!.Length..]
            )
        ];
    }

    public async Task ResetAsync(
        string? pattern = null,
        bool isInstance = true,
        CancellationToken cancellationToken = default
    )
    {
        if (!_redisOptions.Enabled)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(pattern))
        {
            await GetDatabase()
                .ScriptEvaluateAsync(
                    _clearCacheLuaScript,
                    values:
                    [
                        _redisOptions.InstanceName + "*"
                    ]
                );
        }
        else if (isInstance)
        {
            await GetDatabase()
                .ScriptEvaluateAsync(
                    _clearCacheLuaScript,
                    values:
                    [
                        _redisOptions.InstanceName + pattern
                    ]
                );
        }
        else
        {
            await GetDatabase()
                .ScriptEvaluateAsync(
                    _clearCacheLuaScript,
                    values:
                    [
                        pattern
                    ]
                );
        }
    }

    public async Task<bool> AcquireLockAsync(
        string lockKey,
        string lockValue,
        TimeSpan expiration
    )
    {
        return await GetDatabase()
            .StringSetAsync(
                lockKey,
                lockValue,
                expiration,
                When.NotExists
            );
    }

    public async Task<bool> ReleaseLockAsync(
        string lockKey,
        string lockValue
    )
    {
        const string script = @"
            if redis.call('get', KEYS[1]) == ARGV[1] then
                return redis.call('del', KEYS[1])
            else
                return 0
            end";

        var result = (int)await GetDatabase()
            .ScriptEvaluateAsync(
                script,
                [lockKey],
                [lockValue]
            );

        return result == 1;
    }

    public IEnumerable<string?> GetByPattern(
        string? pattern
    )
    {
        if (!_redisOptions.Enabled || string.IsNullOrWhiteSpace(pattern))
        {
            return [];
        }

        var results = GetDatabase()
            .ScriptEvaluate(
                _getPatternLuaScript,
                values: [pattern]
            );
        if (results.Type != ResultType.MultiBulk)
        {
            return [];
        }

        return ((RedisResult[])results!)
            .Select(x => x.ToString())
            .Where(x => !string.IsNullOrEmpty(x));
    }

    public IEnumerable<TDto> GetByPatterns<TDto>(
        string[] patterns
    )
    {
        if (!_redisOptions.Enabled || patterns is not { Length: > 0 })
        {
            return [];
        }

        var data = new List<TDto>();

        var luaScript = LuaScriptLoader.Load("GetHashFieldDataByPatternsWithScan");

        var redisPatterns = patterns
            .Select(pattern => $"{_redisOptions.InstanceName}{pattern}")
            .ToArray();

        var redisResult = GetDatabase()
            .ScriptEvaluate(
                luaScript,
                null,
                [.. redisPatterns]
            );

        var dataResult = (RedisResult[])redisResult!;

        foreach (var item in dataResult)
        {
            var itemString = item.ToString();
            if (string.IsNullOrWhiteSpace(itemString) || string.IsNullOrEmpty(itemString))
            {
                continue;
            }

            var deserializedData = CacheHelper.Deserialize<List<TDto>>(itemString);
            if (deserializedData is not null)
            {
                data.AddRange(deserializedData);
            }
        }

        return data;
    }

    public async Task SetIgnoreInstanceAsync<T>(
        string? key,
        T value,
        int? ttl = null,
        CancellationToken cancellationToken = default
    )
    {
        if (!_redisOptions.Enabled || string.IsNullOrWhiteSpace(key))
        {
            return;
        }

        var cacheValue = CacheHelper.Serialize(value) ?? string.Empty;

        await GetDatabase()
            .HashSetAsync(
                key,
                "data",
                cacheValue
            );
        var timeToLive = ttl ?? _redisOptions.RedisDefaultSlidingExpirationInSecond + Random.Shared.Next(0, 50);

        await GetDatabase()
            .KeyExpireAsync(
                key,
                TimeSpan.FromSeconds(
                    timeToLive
                )
            );
    }

    public async Task<T?> ExecuteLuaByNameWithResultAsync<T>(
        string scriptName,
        string[]? keys = null,
        RedisValue[]? values = null
    )
    {
        if (!_redisOptions.Enabled)
            return default;

        if (keys == null || keys.Length == 0)
            return default;

        var redisKeys = keys
            .Select(k => (RedisKey)$"{_redisOptions.InstanceName}{k}")
            .ToArray();

        var script = LuaScriptLoader.Load(scriptName);

        var result = await GetDatabase()
            .ScriptEvaluateAsync(
                script,
                redisKeys,
                values
            );

        if (result.IsNull)
            return default;

        var resultString = result.ToString();

        if (string.IsNullOrEmpty(resultString))
            return default;

        return CacheHelper.Deserialize<T>(resultString);
    }
}
