using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Cache.Inventory;
using SharedKernel.Cache.Services;
using StackExchange.Redis;

namespace SharedKernel.Cache.Test.Integration;

[Collection(RedisIntegrationCollection.CollectionName)]
public abstract class RedisIntegrationTestBase(
    RedisContainerFixture fixture
) : IAsyncLifetime
{
    private ServiceProvider _serviceProvider = null!;

    protected RedisContainerFixture Fixture { get; } = fixture;

    protected ICacheService CacheService => _serviceProvider.GetRequiredService<ICacheService>();

    protected IRedisCacheService RedisCacheService => _serviceProvider.GetRequiredService<IRedisCacheService>();

    protected IRedisInventoryService RedisInventoryService => _serviceProvider.GetRequiredService<IRedisInventoryService>();

    public virtual async Task InitializeAsync()
    {
        await Fixture.ResetAsync();
        _serviceProvider = Fixture.CreateServiceProvider();
    }

    public virtual async Task DisposeAsync()
    {
        await _serviceProvider.DisposeAsync();
    }

    protected IDatabase GetDatabase(
        int database = 0
    )
    {
        return Fixture.GetDatabase(database);
    }

    protected static string Prefixed(
        string key
    )
    {
        return RedisContainerFixture.Prefixed(key);
    }
}
