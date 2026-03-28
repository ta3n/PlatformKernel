using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Cache.Inventory;
using SharedKernel.Cache.Options;
using SharedKernel.Cache.Services;
using SharedKernel.Cache.Utils;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Implementations;

namespace SharedKernel.Cache;

/// <summary>
/// Provides extension methods for adding distributed caching functionality
/// using Redis to an application.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Configures and registers distributed cache services within the dependency injection container.
    /// Includes configuration for Redis as the distributed cache provider.
    /// </summary>
    /// <param name="services">The IServiceCollection to which the distributed cache services will be added.</param>
    /// <param name="configuration">The application configuration containing the Redis cache settings.</param>
    /// <returns>The updated IServiceCollection with the distributed cache services added.</returns>
    public static IServiceCollection AddDistributedCache(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var section = configuration.GetSection("Redis");
        var redisCacheOptions = section.Get<CacheOptions>();

        services.Configure<CacheOptions>(section);

        if (redisCacheOptions is null)
        {
            return services;
        }

        AddRedisConnectionPoolManager(
            services,
            redisCacheOptions,
            false
        );

        services.AddSingleton<RedisConnectionPool>();
        services.AddSingleton<IRedisCacheService, RedisCacheService>();
        services.AddSingleton<IRedisInventoryService, RedisInventoryService>();

        services.AddSingleton<ICacheService, CacheService>();
        services.AddSingleton<ISingleFlightCacheService, SingleFlightCacheService>();

        services.AddStackExchangeRedisCache(
            options =>
            {
                options.Configuration = redisCacheOptions.UrlConfiguration;
                options.InstanceName = redisCacheOptions.InstanceName;
                options.ConfigurationOptions = redisCacheOptions.ToExtensionConfigureOptions();
            }
        );

        return services;
    }

    /// <summary>
    /// Adds a Redis connection pool manager to the service collection.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to which the Redis connection pool manager is added.
    /// </param>
    /// <param name="redisCacheOptions">
    /// The configuration options used to set up the Redis connection pool manager.
    /// </param>
    /// <param name="isUse">
    /// Determines whether the Redis connection pool manager should be registered. Default is true.
    /// </param>
    private static void AddRedisConnectionPoolManager(
        IServiceCollection services,
        CacheOptions redisCacheOptions,
        bool isUse = true
    )
    {
        if (!isUse)
        {
            return;
        }

        var redisConfiguration = redisCacheOptions.ToExtensionRedisConfiguration();

        services.AddSingleton(redisConfiguration);
        services.AddSingleton(
            _ => new RedisConnectionPoolManager(redisConfiguration)
        );
        services.AddSingleton<IConnectionMultiplexer>(
            _ => new RedisConnectionPoolManager(
                redisConfiguration
            ).GetConnection()
        );
        services.AddSingleton<IConnectionMultiplexer>(
            sp =>
                sp.GetRequiredService<RedisConnectionPoolManager>().GetConnection()
        );
    }
}
