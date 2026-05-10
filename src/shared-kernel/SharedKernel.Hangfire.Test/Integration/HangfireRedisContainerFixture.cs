using Testcontainers.Redis;

namespace SharedKernel.Hangfire.Test.Integration;

public sealed class HangfireRedisContainerFixture : IAsyncLifetime
{
    public const string ExternalConnectionStringEnvironmentVariable =
        "HANGFIRE_TEST_REDIS_CONNECTION";

    private readonly RedisContainer? _redisContainer;
    private readonly bool _useExternalRedis;

    private StackExchange.Redis.ConnectionMultiplexer _adminConnection = null!;

    public HangfireRedisContainerFixture()
    {
        var externalConnectionString = Environment.GetEnvironmentVariable(
            ExternalConnectionStringEnvironmentVariable
        );

        if (!string.IsNullOrWhiteSpace(externalConnectionString))
        {
            _useExternalRedis = true;
            ConnectionString = externalConnectionString;
            return;
        }

        _redisContainer = new RedisBuilder()
            .WithImage("redis:7.2-alpine")
            .Build();
        ConnectionString = string.Empty;
    }

    public string ConnectionString { get; private set; }

    public async Task InitializeAsync()
    {
        if (!_useExternalRedis)
        {
            await _redisContainer!.StartAsync();
            ConnectionString = _redisContainer.GetConnectionString();
        }

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

        if (_redisContainer is not null)
        {
            await _redisContainer.DisposeAsync();
        }
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
