using SharedKernel.Cache.Models;
using StackExchange.Redis;

namespace SharedKernel.Cache.Test.Integration;

public sealed class RedisCacheServiceIntegrationTests(
    RedisContainerFixture fixture
) : RedisIntegrationTestBase(fixture)
{
    [Fact]
    public async Task StringOperations_SupportDatabaseSelection_AndIgnoreInstance()
    {
        var payload = new TestCachePayload("redis", 1);

        Assert.Equal("shared-kernel-cache-test:raw:key", RedisCacheService.BuildKey("raw:key"));

        await RedisCacheService.StringSetAsync(
            "raw:key",
            payload,
            new RedisCacheCommandOptions
            {
                Database = 1
            }
        );
        await RedisCacheService.StringSetAsync(
            "global:key",
            "global-value",
            new RedisCacheCommandOptions
            {
                IgnoreInstanceName = true
            }
        );

        Assert.Null(await RedisCacheService.StringGetAsync<TestCachePayload>("raw:key"));
        Assert.Equal(
            payload,
            await RedisCacheService.StringGetAsync<TestCachePayload>(
                "raw:key",
                new RedisCacheCommandOptions
                {
                    Database = 1
                }
            )
        );
        Assert.Equal("global-value", await RedisCacheService.StringGetAsync(
            "global:key",
            new RedisCacheCommandOptions
            {
                IgnoreInstanceName = true
            }));
        Assert.Equal("global-value", await GetDatabase().StringGetAsync("global:key"));
    }

    [Fact]
    public async Task StringGetMany_SetBulk_AndIncrement_Work()
    {
        await RedisCacheService.StringSetBulkAsync(
            new Dictionary<string, string>
            {
                ["bulk:key:1"] = "one",
                ["bulk:key:2"] = "two"
            },
            new RedisCacheCommandOptions
            {
                Expiry = TimeSpan.FromSeconds(30)
            }
        );

        var values = await RedisCacheService.StringGetManyAsync(["bulk:key:1", "bulk:key:2", "bulk:key:missing"]);

        Assert.Equal("one", values["bulk:key:1"]);
        Assert.Equal("two", values["bulk:key:2"]);
        Assert.Null(values["bulk:key:missing"]);

        var counter = await RedisCacheService.StringIncrementAsync("counter:key", 2);

        Assert.Equal(2, counter);
        Assert.Equal("2", await RedisCacheService.StringGetAsync("counter:key"));
    }

    [Fact]
    public async Task HashAndKeyOperations_Work()
    {
        await RedisCacheService.HashSetAsync(
            "hash:key",
            "payload",
            new TestCachePayload("hash", 5),
            new RedisCacheCommandOptions
            {
                Expiry = TimeSpan.FromSeconds(5)
            }
        );

        var payload = await RedisCacheService.HashGetAsync<TestCachePayload>("hash:key", "payload");
        var values = await RedisCacheService.HashGetAllAsync("hash:key");

        Assert.Equal(new TestCachePayload("hash", 5), payload);
        Assert.Single(values);
        Assert.NotNull(values["payload"]);
        Assert.Contains("hash", values["payload"]!, StringComparison.Ordinal);

        Assert.True(await RedisCacheService.KeyExpireAsync("hash:key", TimeSpan.FromSeconds(1)));
        await Task.Delay(TimeSpan.FromMilliseconds(1200));
        Assert.False(await GetDatabase().KeyExistsAsync(Prefixed("hash:key")));

        await RedisCacheService.StringSetAsync("delete:key", "value");
        Assert.True(await RedisCacheService.KeyDeleteAsync("delete:key"));
        Assert.Null(await RedisCacheService.StringGetAsync("delete:key"));
    }

    [Fact]
    public async Task PatternQueries_AndPatternDeletion_Work()
    {
        await RedisCacheService.StringSetAsync("pattern:1", "one");
        await RedisCacheService.StringSetAsync("pattern:2", "two");
        await RedisCacheService.StringSetAsync(
            "shared:pattern",
            "shared",
            new RedisCacheCommandOptions
            {
                IgnoreInstanceName = true
            }
        );

        var prefixedKeys = (await RedisCacheService.GetKeysAsync("pattern:*"))
            .OrderBy(static key => key)
            .ToArray();
        var removed = await RedisCacheService.RemoveByPatternAsync("pattern:*");

        Assert.Equal(["pattern:1", "pattern:2"], prefixedKeys);
        Assert.Equal(2, removed);
        Assert.Empty(await RedisCacheService.GetKeysAsync("pattern:*"));
        Assert.Equal("shared", await RedisCacheService.StringGetAsync(
            "shared:pattern",
            new RedisCacheCommandOptions
            {
                IgnoreInstanceName = true
            }));
    }

    [Fact]
    public async Task LockAndLuaOperations_Work()
    {
        Assert.True(await RedisCacheService.AcquireLockAsync("redis:lock", "owner-1", TimeSpan.FromSeconds(30)));
        Assert.False(await RedisCacheService.AcquireLockAsync("redis:lock", "owner-2", TimeSpan.FromSeconds(30)));
        Assert.False(await RedisCacheService.ReleaseLockAsync("redis:lock", "owner-2"));
        Assert.True(await RedisCacheService.ReleaseLockAsync("redis:lock", "owner-1"));

        var result = await RedisCacheService.ExecuteLuaByNameWithResultAsync<int>(
            "AcquirePriceCalendarRangeLock",
            ["lua:calendar"],
            [
                20260324,
                20260325,
                "lua-token",
                30
            ],
            new RedisCacheCommandOptions
            {
                Database = 1
            }
        );

        Assert.Equal(1, result);
        Assert.True(await GetDatabase(1).KeyExistsAsync(Prefixed("lua:calendar")));
    }
}
