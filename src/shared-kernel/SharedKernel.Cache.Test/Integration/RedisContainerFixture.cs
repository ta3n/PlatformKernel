using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Cache.Inventory;
using SharedKernel.Cache.Services;
using StackExchange.Redis;
using Testcontainers.Redis;

namespace SharedKernel.Cache.Test.Integration;

public sealed class RedisContainerFixture : IAsyncLifetime
{
    public const string DefaultInstanceName = "shared-kernel-cache-test:";

    private readonly RedisContainer _redisContainer = new RedisBuilder()
        .WithImage("redis:7.2-alpine")
        .Build();

    private ConnectionMultiplexer _adminConnection = null!;

    public string ConnectionString => _redisContainer.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _redisContainer.StartAsync();

        var configurationOptions = ConfigurationOptions.Parse(ConnectionString);
        configurationOptions.AllowAdmin = true;
        _adminConnection = await ConnectionMultiplexer.ConnectAsync(configurationOptions);
    }

    public async Task DisposeAsync()
    {
        if (_adminConnection is not null)
        {
            await _adminConnection.CloseAsync();
            await _adminConnection.DisposeAsync();
        }

        await _redisContainer.DisposeAsync();
    }

    public async Task ResetAsync()
    {
        foreach (var endpoint in _adminConnection.GetEndPoints())
        {
            var server = _adminConnection.GetServer(endpoint);
            await server.FlushAllDatabasesAsync();
        }
    }

    public ServiceProvider CreateServiceProvider(
        int defaultDatabase = 0,
        string? instanceName = null
    )
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
            [
                new KeyValuePair<string, string?>("Redis:Enabled", "true"),
                new KeyValuePair<string, string?>("Redis:UrlConfiguration", ConnectionString),
                new KeyValuePair<string, string?>("Redis:InstanceName", instanceName ?? DefaultInstanceName),
                new KeyValuePair<string, string?>("Redis:DefaultDatabase", defaultDatabase.ToString(CultureInfo.InvariantCulture)),
                new KeyValuePair<string, string?>("Redis:RedisDefaultSlidingExpirationInSecond", "30")
            ])
            .Build();

        services.AddLogging();
        services.AddOptions();
        services.AddDistributedCache(configuration);

        return services.BuildServiceProvider(validateScopes: true);
    }

    public IDatabase GetDatabase(
        int database = 0
    )
    {
        return _adminConnection.GetDatabase(database);
    }

    public static string Prefixed(
        string key,
        string? instanceName = null
    )
    {
        return $"{instanceName ?? DefaultInstanceName}{key}";
    }
}
