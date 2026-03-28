using SharedKernel.Cache.Models;
using SharedKernel.Cache.Options;
using SharedKernel.Cache.Services;

namespace SharedKernel.Cache.Test.Integration;

public sealed class SingleFlightCacheServiceIntegrationTests(
    RedisContainerFixture fixture
) : RedisIntegrationTestBase(fixture)
{
    [Fact]
    public async Task GetOrCreateAsync_CachesFreshValue_AndReusesIt()
    {
        var factoryCalls = 0;
        var options = new SingleFlightCacheOptions
        {
            FreshTtl = TimeSpan.FromSeconds(10)
        };

        var first = await SingleFlightCacheService.GetOrCreateAsync(
            "singleflight:fresh",
            _ =>
            {
                factoryCalls++;
                return Task.FromResult(new TestCachePayload("fresh", factoryCalls));
            },
            options
        );

        var second = await SingleFlightCacheService.GetOrCreateAsync(
            "singleflight:fresh",
            _ =>
            {
                factoryCalls++;
                return Task.FromResult(new TestCachePayload("fresh", factoryCalls));
            },
            options
        );

        Assert.Equal(1, factoryCalls);
        Assert.Equal(SingleFlightCacheSource.Refreshed, first.Source);
        Assert.Equal(SingleFlightCacheSource.CacheHit, second.Source);
        Assert.Equal(first.Value, second.Value);
    }

    [Fact]
    public async Task GetOrCreateAsync_CoalescesConcurrentMisses()
    {
        var factoryCalls = 0;
        var start = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var options = new SingleFlightCacheOptions
        {
            FreshTtl = TimeSpan.FromSeconds(10),
            WaitTimeout = TimeSpan.FromSeconds(2),
            LockTtl = TimeSpan.FromSeconds(5),
            PollInterval = TimeSpan.FromMilliseconds(50)
        };

        async Task<SingleFlightCacheResult<TestCachePayload>> ExecuteAsync()
        {
            await start.Task;

            return await SingleFlightCacheService.GetOrCreateAsync(
                "singleflight:concurrent",
                async ct =>
                {
                    var attempt = Interlocked.Increment(ref factoryCalls);
                    await Task.Delay(TimeSpan.FromMilliseconds(300), ct);
                    return new TestCachePayload("concurrent", attempt);
                },
                options
            );
        }

        var tasks = Enumerable.Range(0, 4)
            .Select(_ => Task.Run(ExecuteAsync))
            .ToArray();

        start.SetResult();

        var results = await Task.WhenAll(tasks);

        Assert.Equal(1, factoryCalls);
        Assert.Contains(results, result => result.Source == SingleFlightCacheSource.Refreshed);
        Assert.DoesNotContain(results, result => result.Source == SingleFlightCacheSource.OriginFallback);
        Assert.All(results, result => Assert.Equal(new TestCachePayload("concurrent", 1), result.Value));
    }

    [Fact]
    public async Task GetOrCreateAsync_ReturnsStaleValue_WhenWaitTimeoutExpires()
    {
        var seedOptions = new SingleFlightCacheOptions
        {
            FreshTtl = TimeSpan.FromMilliseconds(250),
            StaleTtl = TimeSpan.FromSeconds(5),
            WaitTimeout = TimeSpan.FromSeconds(1),
            LockTtl = TimeSpan.FromSeconds(2),
            PollInterval = TimeSpan.FromMilliseconds(50)
        };

        await SingleFlightCacheService.GetOrCreateAsync(
            "singleflight:stale",
            _ => Task.FromResult(new TestCachePayload("stale", 1)),
            seedOptions
        );

        await Task.Delay(TimeSpan.FromMilliseconds(350));

        var refreshStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var refreshTask = Task.Run(() => SingleFlightCacheService.GetOrCreateAsync(
            "singleflight:stale",
            async ct =>
            {
                refreshStarted.TrySetResult();
                await Task.Delay(TimeSpan.FromMilliseconds(700), ct);
                return new TestCachePayload("stale", 2);
            },
            seedOptions
        ));

        await refreshStarted.Task;

        var staleResult = await SingleFlightCacheService.GetOrCreateAsync(
            "singleflight:stale",
            _ => Task.FromResult(new TestCachePayload("unexpected", 99)),
            new SingleFlightCacheOptions
            {
                FreshTtl = TimeSpan.FromMilliseconds(250),
                StaleTtl = TimeSpan.FromSeconds(5),
                WaitTimeout = TimeSpan.FromMilliseconds(200),
                LockTtl = TimeSpan.FromSeconds(2),
                PollInterval = TimeSpan.FromMilliseconds(50),
                AllowOriginFallback = false
            }
        );

        var refreshed = await refreshTask;

        Assert.Equal(SingleFlightCacheSource.StaleHit, staleResult.Source);
        Assert.Equal(new TestCachePayload("stale", 1), staleResult.Value);
        Assert.Equal(SingleFlightCacheSource.Refreshed, refreshed.Source);
        Assert.Equal(new TestCachePayload("stale", 2), refreshed.Value);
    }

    [Fact]
    public async Task GetOrCreateAsync_FallsBackToOrigin_WhenWaitTimeoutExpires_AndNoStaleExists()
    {
        var factoryCalls = 0;
        var refreshStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var options = new SingleFlightCacheOptions
        {
            FreshTtl = TimeSpan.FromSeconds(10),
            WaitTimeout = TimeSpan.FromMilliseconds(200),
            LockTtl = TimeSpan.FromSeconds(2),
            PollInterval = TimeSpan.FromMilliseconds(50),
            ServeStaleOnWaitTimeout = false,
            AllowOriginFallback = true
        };

        var lockOwnerTask = Task.Run(() => SingleFlightCacheService.GetOrCreateAsync(
            "singleflight:fallback",
            async ct =>
            {
                var attempt = Interlocked.Increment(ref factoryCalls);
                refreshStarted.TrySetResult();
                await Task.Delay(TimeSpan.FromMilliseconds(700), ct);
                return new TestCachePayload("fallback", attempt);
            },
            options
        ));

        await refreshStarted.Task;

        var fallbackResult = await SingleFlightCacheService.GetOrCreateAsync(
            "singleflight:fallback",
            _ =>
            {
                var attempt = Interlocked.Increment(ref factoryCalls);
                return Task.FromResult(new TestCachePayload("fallback", attempt));
            },
            options
        );

        var lockOwnerResult = await lockOwnerTask;

        Assert.Equal(2, factoryCalls);
        Assert.Equal(SingleFlightCacheSource.OriginFallback, fallbackResult.Source);
        Assert.Equal(new TestCachePayload("fallback", 2), fallbackResult.Value);
        Assert.Equal(SingleFlightCacheSource.Refreshed, lockOwnerResult.Source);
        Assert.Equal(new TestCachePayload("fallback", 1), lockOwnerResult.Value);
    }
}
