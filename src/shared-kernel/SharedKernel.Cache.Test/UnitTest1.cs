using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Cache;
using SharedKernel.Cache.Options;
using SharedKernel.Cache.Services;
using SharedKernel.Cache.Utils;

namespace SharedKernel.Cache.Test;

public class UnitTest1
{
    [Fact]
    public void SerializeAndDeserialize_UseSharedJsonSettings()
    {
        var payload = new CachePayload("Kernel", 2);

        var json = CacheHelper.Serialize(payload);
        var restored = CacheHelper.Deserialize<CachePayload>(json!);

        Assert.NotNull(json);
        Assert.Contains("\"name\":\"Kernel\"", json, StringComparison.Ordinal);
        Assert.NotNull(restored);
        Assert.Equal(payload, restored);
    }

    [Fact]
    public void ParseRedisConnectionString_ReturnsHostsPasswordAndOptionalFlag()
    {
        var result = CacheHelper.ParseRedisConnectionString("redis-a:6379,redis-b:6380,password=secret,ssl=true");

        Assert.Equal(2, result.Hosts.Count);
        Assert.Equal("redis-a", result.Hosts[0].Host);
        Assert.Equal(6379, result.Hosts[0].Port);
        Assert.Equal("secret", result.Password);
        Assert.True(result.HasOptionalParameters);
    }

    [Fact]
    public void ToExtensionRedisConfiguration_MapsHostsAndConnectionString()
    {
        var options = new CacheOptions
        {
            UrlConfiguration = "redis-a:6379,password=secret,ssl=true",
            ConnectTimeout = 1234,
            SyncTimeout = 5678,
            PoolSize = 5,
            Ssl = true
        };

        var configuration = options.ToExtensionRedisConfiguration();

        Assert.Equal(1234, configuration.ConnectTimeout);
        Assert.Equal(5678, configuration.SyncTimeout);
        Assert.Equal(5, configuration.PoolSize);
        Assert.True(configuration.Ssl);
        Assert.Equal("secret", configuration.Password);
        Assert.Equal("redis-a", configuration.Hosts.Single().Host);
        Assert.Equal("redis-a:6379,password=secret,ssl=true", configuration.ConnectionString);
    }

    [Fact]
    public void AddDistributedCache_RegistersCacheServicesWhenRedisSectionExists()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
            [
                new KeyValuePair<string, string?>("Redis:Enabled", "true"),
                new KeyValuePair<string, string?>("Redis:UrlConfiguration", "localhost:6379"),
                new KeyValuePair<string, string?>("Redis:InstanceName", "kernel:")
            ])
            .Build();

        services.AddLogging();
        services.AddOptions();
        services.AddDistributedCache(configuration);

        Assert.Contains(services, descriptor => descriptor.ServiceType == typeof(ICacheService));
        Assert.Contains(services, descriptor => descriptor.ServiceType.FullName == "Microsoft.Extensions.Caching.Distributed.IDistributedCache");
    }

    [Fact]
    public void ComputeHash_ReturnsDeterministicLowerCaseValue()
    {
        var first = CacheHelper.ComputeHash(["shared", "kernel"]);
        var second = CacheHelper.ComputeHash(["shared", "kernel"]);

        Assert.Equal(first, second);
        Assert.Equal(16, first.Length);
        Assert.Equal(first, first.ToLowerInvariant());
    }

    private sealed record CachePayload(string Name, int Version);
}
