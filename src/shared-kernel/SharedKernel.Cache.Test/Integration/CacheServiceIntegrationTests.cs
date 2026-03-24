using Microsoft.Extensions.Caching.Distributed;
using SharedKernel.Cache.Options;
using SharedKernel.Cache.Utils;

namespace SharedKernel.Cache.Test.Integration;

public sealed class CacheServiceIntegrationTests(
    RedisContainerFixture fixture
) : RedisIntegrationTestBase(fixture)
{
    [Fact]
    public void GetDatabase_CanTargetRequestedDatabase()
    {
        var cacheService = CacheService;

        cacheService.GetDatabase(2).StringSet("manual:key", "value");

        Assert.False(cacheService.GetDatabase().KeyExists("manual:key"));
        Assert.True(cacheService.GetDatabase(2).KeyExists("manual:key"));
    }

    [Fact]
    public async Task SetOptionSetting_WhenDisabled_SkipsWrites()
    {
        CacheService.SetOptionSetting(
            new CacheOptions
            {
                Enabled = false,
                InstanceName = RedisContainerFixture.DefaultInstanceName,
                DefaultDatabase = 0,
                RedisDefaultSlidingExpirationInSecond = 30
            }
        );

        await CacheService.SetAsync("disabled:key", new TestCachePayload("disabled", 1));

        Assert.Null(await CacheService.GetAsync<TestCachePayload>("disabled:key"));
        Assert.False(await GetDatabase().KeyExistsAsync(Prefixed("disabled:key")));
    }

    [Fact]
    public void DistributedCacheSyncOperations_Work()
    {
        var key = "distributed:key";
        var payload = new TestCachePayload("kernel", 2);

        CacheService.Set(key, payload);
        Assert.Equal(payload, CacheService.Get<TestCachePayload>(key));
        CacheService.Remove(key);
        Assert.Null(CacheService.Get<TestCachePayload>(key));
    }

    [Fact]
    public async Task DistributedCacheAsyncOperations_WorkEndToEnd()
    {
        var key = "distributed:key";
        var payload = new TestCachePayload("kernel", 2);

        await CacheService.SetAsync(key, payload);
        Assert.Equal(payload, await CacheService.GetAsync<TestCachePayload>(key));
        Assert.NotNull(await CacheService.GetStringAsync(key));

        await CacheService.RemoveAsync(key);
        Assert.Null(await CacheService.GetAsync<TestCachePayload>(key));

        await CacheService.SetAsync(
            key,
            payload,
            new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromSeconds(2)
            }
        );

        await Task.Delay(TimeSpan.FromSeconds(1));
        await CacheService.RefreshAsync(key);
        await Task.Delay(TimeSpan.FromMilliseconds(1500));

        Assert.Equal(payload, await CacheService.GetAsync<TestCachePayload>(key));
    }

    [Fact]
    public async Task BulkHashOperations_AndPatternQueries_Work()
    {
        await CacheService.SetBulkAsync(
            new Dictionary<string, List<TestCacheListItem>>
            {
                ["bulk:1"] = [new("A1"), new("A2")],
                ["bulk:2"] = [new("B1")]
            }
        );
        await CacheService.SetBulkAsync(
            new Dictionary<string, List<TestCacheListItem>>
            {
                ["bulk:ttl"] = [new("TTL")]
            },
            ttlcustom: 5
        );

        var keys = (await CacheService.GetKeysAsync("bulk:*"))
            .OrderBy(static key => key)
            .ToArray();
        var data = CacheService.GetByPatterns<TestCacheListItem>(["bulk:*"]).ToArray();
        var ttl = await GetDatabase().KeyTimeToLiveAsync(Prefixed("bulk:ttl"));

        Assert.Equal(["bulk:1", "bulk:2", "bulk:ttl"], keys);
        Assert.Equal(4, data.Length);
        Assert.Contains(data, item => item.Code == "TTL");
        Assert.True(ttl.HasValue);
        Assert.InRange(ttl.Value.TotalSeconds, 1, 5);
    }

    [Fact]
    public void SynchronousPatternOperations_ClearPrefixedKeys()
    {
        var database = GetDatabase();

        database.HashSet(
            Prefixed("remove:1"),
            "data",
            CacheHelper.Serialize(new List<TestCacheListItem> { new("R1") }));
        database.HashSet(
            Prefixed("remove:2"),
            "data",
            CacheHelper.Serialize(new List<TestCacheListItem> { new("R2") }));
        database.HashSet(
            Prefixed("remove:3"),
            "data",
            CacheHelper.Serialize(new List<TestCacheListItem> { new("R3") }));

        Assert.True(CacheService.RemoveByPattern("remove:1"));
        Assert.False(database.KeyExists(Prefixed("remove:1")));

        CacheService.RemoveByPatterns(false, "remove:2");
        Assert.False(database.KeyExists(Prefixed("remove:2")));

        CacheService.Reset("remove:3");
        Assert.False(database.KeyExists(Prefixed("remove:3")));
    }

    [Fact]
    public async Task AsyncPatternOperations_ClearPrefixedKeys()
    {
        await CacheService.SetBulkAsync(
            new Dictionary<string, List<TestCacheListItem>>
            {
                ["remove:async:1"] = [new("R1")],
                ["remove:async:2"] = [new("R2")]
            }
        );

        await CacheService.RemoveByPatternsAsync(false, "remove:async:1");
        Assert.Empty(await CacheService.GetKeysAsync("remove:async:1"));

        await CacheService.SetBulkAsync(
            new Dictionary<string, List<TestCacheListItem>>
            {
                ["reset:async:1"] = [new("X1")],
                ["reset:async:2"] = [new("X2")]
            }
        );

        await CacheService.ResetAsync("reset:async:*");
        Assert.Empty(await CacheService.GetKeysAsync("reset:async:*"));
    }

    [Fact]
    public async Task IgnoreInstanceQueries_AndNonInstanceReset_Work()
    {
        await CacheService.SetIgnoreInstanceAsync(
            "shared:1",
            new TestCachePayload("shared", 1),
            ttl: 30
        );
        await CacheService.SetIgnoreInstanceAsync(
            "shared:2",
            new TestCachePayload("shared", 2),
            ttl: 30
        );

        var values = CacheService.GetByPattern("shared:*").ToArray();

        Assert.Equal(2, values.Length);
        Assert.All(
            values,
            value =>
            {
                Assert.NotNull(value);
                Assert.Contains("shared", value!, StringComparison.Ordinal);
            }
        );

        await CacheService.RemoveByPatternsAsync(true, "shared:1");
        Assert.Single(CacheService.GetByPattern("shared:*"));
        await CacheService.ResetAsync("shared:*", isInstance: false);
        Assert.Empty(CacheService.GetByPattern("shared:*"));
    }

    [Fact]
    public async Task LockAndLuaOperations_Work()
    {
        Assert.True(await CacheService.AcquireLockAsync("lock:1", "owner-1", TimeSpan.FromSeconds(30)));
        Assert.False(await CacheService.AcquireLockAsync("lock:1", "owner-2", TimeSpan.FromSeconds(30)));
        Assert.False(await CacheService.ReleaseLockAsync("lock:1", "owner-2"));
        Assert.True(await CacheService.ReleaseLockAsync("lock:1", "owner-1"));

        var acquired = await CacheService.ExecuteLuaByNameWithResultAsync<int>(
            "AcquirePriceCalendarRangeLock",
            ["calendar:1"],
            [
                20260324,
                20260326,
                "token-1",
                30
            ]
        );
        var overlapped = await CacheService.ExecuteLuaByNameWithResultAsync<int>(
            "AcquirePriceCalendarRangeLock",
            ["calendar:1"],
            [
                20260325,
                20260327,
                "token-2",
                30
            ]
        );
        var released = await CacheService.ExecuteLuaByNameWithResultAsync<int>(
            "ReleasePriceCalendarRangeLock",
            ["calendar:1"],
            [
                "token-1"
            ]
        );

        Assert.Equal(1, acquired);
        Assert.Equal(0, overlapped);
        Assert.Equal(1, released);
    }
}
