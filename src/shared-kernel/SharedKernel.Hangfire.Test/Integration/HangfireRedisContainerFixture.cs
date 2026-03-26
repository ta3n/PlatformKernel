using Testcontainers.Redis;

namespace SharedKernel.Hangfire.Test.Integration;

public sealed class HangfireRedisContainerFixture : IAsyncLifetime
{
    private readonly RedisContainer _redisContainer = new RedisBuilder()
        .WithImage("redis:7.2-alpine")
        .Build();

    private StackExchange.Redis.ConnectionMultiplexer _adminConnection = null!;

    public string ConnectionString => _redisContainer.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _redisContainer.StartAsync();

        var configurationOptions = StackExchange.Redis.ConfigurationOptions.Parse(ConnectionString);
        configurationOptions.AllowAdmin = true;
        configurationOptions.AbortOnConnectFail = false;

        _adminConnection = await StackExchange.Redis.ConnectionMultiplexer.ConnectAsync(configurationOptions);
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

    public HangfireScaleOutWebApplicationFactory CreateFactory(
        string instanceId,
        string hangfirePrefix,
        string statePrefix
    )
    {
        return new HangfireScaleOutWebApplicationFactory(
            ConnectionString,
            instanceId,
            hangfirePrefix,
            statePrefix
        );
    }
}
