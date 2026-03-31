using System.Diagnostics;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using SharedKernel.Cache.Models;
using SharedKernel.Cache.Options;
using SharedKernel.Cache.Utils;

namespace SharedKernel.Cache.Services;

/// <summary>
/// Provides a distributed single-flight cache helper that uses Redis locks to coordinate refreshes across nodes.
/// </summary>
public sealed class SingleFlightCacheService(
    ILogger<SingleFlightCacheService> logger,
    ICacheService cacheService
) : ISingleFlightCacheService
{
    private const string LockKeyPrefix = "singleflight:lock:";
    private const string StaleKeyPrefix = "singleflight:stale:";

    public async Task<SingleFlightCacheResult<T>> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> valueFactory,
        SingleFlightCacheOptions? options = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(valueFactory);

        if (!cacheService.IsEnabled)
        {
            var bypassed = await valueFactory(cancellationToken);
            return new SingleFlightCacheResult<T>(
                bypassed,
                SingleFlightCacheSource.Bypassed,
                TimeSpan.Zero
            );
        }

        var normalizedOptions = NormalizeOptions(options);
        var lockKey = string.IsNullOrWhiteSpace(normalizedOptions.LockKey)
            ? $"{LockKeyPrefix}{key}"
            : normalizedOptions.LockKey;
        var staleKey = string.IsNullOrWhiteSpace(normalizedOptions.StaleKey)
            ? $"{StaleKeyPrefix}{key}"
            : normalizedOptions.StaleKey;

        var freshValue = await TryGetAsync<T>(
            key,
            cancellationToken
        );
        if (freshValue.Found)
        {
            return new SingleFlightCacheResult<T>(
                freshValue.Value,
                SingleFlightCacheSource.CacheHit,
                TimeSpan.Zero
            );
        }

        var lockId = Guid.NewGuid().ToString("N");
        var acquired = await cacheService.AcquireLockAsync(
            lockKey,
            lockId,
            normalizedOptions.LockTtl
        );

        if (acquired)
        {
            try
            {
                var cachedAfterLock = await TryGetAsync<T>(
                    key,
                    cancellationToken
                );
                if (cachedAfterLock.Found)
                {
                    return new SingleFlightCacheResult<T>(
                        cachedAfterLock.Value,
                        SingleFlightCacheSource.CacheHit,
                        TimeSpan.Zero
                    );
                }

                var refreshedValue = await valueFactory(cancellationToken);
                await StoreAsync(
                    key,
                    staleKey,
                    refreshedValue,
                    normalizedOptions,
                    cancellationToken
                );

                return new SingleFlightCacheResult<T>(
                    refreshedValue,
                    SingleFlightCacheSource.Refreshed,
                    TimeSpan.Zero
                );
            }
            finally
            {
                try
                {
                    await cacheService.ReleaseLockAsync(
                        lockKey,
                        lockId
                    );
                }
                catch (Exception ex)
                {
                    logger.LogWarning(
                        ex,
                        "Failed to release single-flight lock for cache key {CacheKey}",
                        key
                    );
                }
            }
        }

        var stopwatch = Stopwatch.StartNew();
        if (normalizedOptions.WaitTimeout > TimeSpan.Zero)
        {
            while (stopwatch.Elapsed < normalizedOptions.WaitTimeout)
            {
                var remaining = normalizedOptions.WaitTimeout - stopwatch.Elapsed;
                if (remaining <= TimeSpan.Zero)
                {
                    break;
                }

                var delay = normalizedOptions.PollInterval <= remaining
                    ? normalizedOptions.PollInterval
                    : remaining;

                await Task.Delay(
                    delay,
                    cancellationToken
                );

                var waitedValue = await TryGetAsync<T>(
                    key,
                    cancellationToken
                );
                if (waitedValue.Found)
                {
                    return new SingleFlightCacheResult<T>(
                        waitedValue.Value,
                        SingleFlightCacheSource.WaitedCacheHit,
                        stopwatch.Elapsed
                    );
                }
            }
        }

        var lastChanceFreshValue = await TryGetAsync<T>(
            key,
            cancellationToken
        );
        if (lastChanceFreshValue.Found)
        {
            return new SingleFlightCacheResult<T>(
                lastChanceFreshValue.Value,
                SingleFlightCacheSource.WaitedCacheHit,
                stopwatch.Elapsed
            );
        }

        if (normalizedOptions.ServeStaleOnWaitTimeout && normalizedOptions.StaleTtl.HasValue)
        {
            var staleValue = await TryGetAsync<T>(
                staleKey,
                cancellationToken
            );
            if (staleValue.Found)
            {
                return new SingleFlightCacheResult<T>(
                    staleValue.Value,
                    SingleFlightCacheSource.StaleHit,
                    stopwatch.Elapsed
                );
            }
        }

        if (!normalizedOptions.AllowOriginFallback)
        {
            throw new TimeoutException(
                $"Timed out waiting for cache key '{key}' to be refreshed."
            );
        }

        var fallbackValue = await valueFactory(cancellationToken);
        if (normalizedOptions.CacheOriginFallbackResult)
        {
            await StoreAsync(
                key,
                staleKey,
                fallbackValue,
                normalizedOptions,
                cancellationToken
            );
        }

        return new SingleFlightCacheResult<T>(
            fallbackValue,
            SingleFlightCacheSource.OriginFallback,
            stopwatch.Elapsed
        );
    }

    private static SingleFlightCacheOptions NormalizeOptions(
        SingleFlightCacheOptions? options
    )
    {
        var normalized = options ?? new SingleFlightCacheOptions();

        if (normalized.FreshTtl is { } freshTtl && freshTtl <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(options),
                "FreshTtl must be greater than zero."
            );
        }

        if (normalized.StaleTtl is { } staleTtl && staleTtl <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(options),
                "StaleTtl must be greater than zero."
            );
        }

        if (normalized.WaitTimeout < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(options),
                "WaitTimeout must be greater than or equal to zero."
            );
        }

        if (normalized.PollInterval <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(options),
                "PollInterval must be greater than zero."
            );
        }

        if (normalized.LockTtl <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(options),
                "LockTtl must be greater than zero."
            );
        }

        if (normalized.WaitTimeout > TimeSpan.Zero && normalized.LockTtl < normalized.WaitTimeout)
        {
            throw new ArgumentOutOfRangeException(
                nameof(options),
                "LockTtl must be greater than or equal to WaitTimeout."
            );
        }

        return normalized;
    }

    private async Task<(bool Found, T? Value)> TryGetAsync<T>(
        string key,
        CancellationToken cancellationToken
    )
    {
        var rawValue = await cacheService.GetStringAsync(
            key,
            cancellationToken
        );

        if (string.IsNullOrWhiteSpace(rawValue))
        {
            return (false, default);
        }

        return (true, CacheHelper.Deserialize<T>(rawValue));
    }

    private async Task StoreAsync<T>(
        string key,
        string staleKey,
        T value,
        SingleFlightCacheOptions options,
        CancellationToken cancellationToken
    )
    {
        if (value is null)
        {
            return;
        }

        if (options.FreshTtl is { } freshTtl)
        {
            await cacheService.SetAsync(
                key,
                value,
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = freshTtl },
                cancellationToken
            );
        }
        else
        {
            await cacheService.SetAsync(
                key,
                value,
                cancellationToken
            );
        }

        if (options.StaleTtl is not { } staleTtl)
        {
            return;
        }

        await cacheService.SetAsync(
            staleKey,
            value,
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = staleTtl },
            cancellationToken
        );
    }
}
