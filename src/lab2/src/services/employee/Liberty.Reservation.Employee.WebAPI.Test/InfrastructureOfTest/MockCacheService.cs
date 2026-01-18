using Liberty.Cache.Options;
using Liberty.Cache.Services;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using StackExchange.Redis;

namespace Liberty.Reservation.Employee.WebAPI.Test.InfrastructureOfTest;

public class MockCacheService : ICacheService
{
    public bool IsEnabled => false;

    public IDatabase GetDatabase()
    {
        return new Mock<IDatabase>().Object;
    }

    public void SetOptionSetting(
        CacheOptions cacheOptions
    )
    {
    }

    public T? Get<T>(
        string key
    )
    {
        return default;
    }

    public void Set<T>(
        string key,
        T value
    )
    {
    }

    public void Set<T>(
        string key,
        T value,
        DistributedCacheEntryOptions distributedCacheEntryOptions
    )
    {
    }

    public void Remove(
        string key
    )
    {
    }

    public bool RemoveByPattern(
        string pattern
    )
    {
        return true;
    }

    public void RemoveByPatterns(
        bool ignoreInstance,
        params string[] patterns
    )
    {
    }

    public Task RemoveByPatternsAsync(
        bool ignoreInstance,
        params string[] patterns
    )
    {
        return Task.CompletedTask;
    }

    public void Reset(
        string? pattern
    )
    {
    }

    public Task<T?> GetAsync<T>(
        string key,
        CancellationToken cancellationToken = default
    )
    {
        return Task.FromResult(default(T?));
    }

    public Task<string?> GetStringAsync(
        string key,
        CancellationToken cancellationToken = default
    )
    {
        return Task.FromResult<string?>(null);
    }

    public Task SetAsync<T>(
        string key,
        T value,
        CancellationToken cancellationToken = default
    )
    {
        return Task.FromResult(Task.CompletedTask);
    }

    public Task SetAsync<T>(
        string key,
        T value,
        DistributedCacheEntryOptions distributedCacheEntryOptions,
        CancellationToken cancellationToken = default
    )
    {
        return Task.FromResult(Task.CompletedTask);
    }

    public Task SetBulkAsync<T>(
        Dictionary<string, T> keyValuePairs,
        CancellationToken cancellationToken = default
    )
    {
        return Task.FromResult(Task.CompletedTask);
    }

    public Task SetBulkAsync<T>(
        Dictionary<string, T> keyValuePairs,
        int? ttlcustom,
        CancellationToken cancellationToken = default
    )
    {
        return Task.FromResult(Task.CompletedTask);
    }

    public Task RemoveAsync(
        string key,
        CancellationToken cancellationToken = default
    )
    {
        return Task.FromResult(Task.CompletedTask);
    }

    public Task RefreshAsync(
        string key,
        CancellationToken cancellationToken = default
    )
    {
        return Task.FromResult(Task.CompletedTask);
    }

    public Task<IEnumerable<string>> GetKeysAsync(
        string pattern,
        CancellationToken cancellationToken = default
    )
    {
        return Task.FromResult(Enumerable.Empty<string>());
    }

    public Task ResetAsync(
        string? pattern = null,
        bool isInstance = true,
        CancellationToken cancellationToken = default
    )
    {
        return Task.FromResult(Task.CompletedTask);
    }

    public Task<bool> AcquireLockAsync(
        string lockKey,
        string lockValue,
        TimeSpan expiration
    )
    {
        return Task.FromResult(true);
    }

    public Task<bool> ReleaseLockAsync(
        string lockKey,
        string lockValue
    )
    {
        return Task.FromResult(true);
    }

    public IEnumerable<string?> GetByPattern(
        string? pattern
    )
    {
        return [];
    }

    public IEnumerable<TDto> GetByPatterns<TDto>(
        string[] patterns
    )
    {
        return [];
    }

    public Task SetIgnoreInstanceAsync<T>(
        string? key,
        T value,
        int? ttl = null,
        CancellationToken cancellationToken = default
    )
    {
        return Task.FromResult(Task.CompletedTask);
    }

    public Task<T?> ExecuteLuaByNameWithResultAsync<T>(
        string scriptName,
        string[]? keys = null,
        RedisValue[]? values = null
    )
    {
        return Task.FromResult(default(T?));
    }
}
