using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace SharedKernel.Hangfire.Test.Integration;

public sealed class HangfireScaleOutWebApplicationFactory
    : WebApplicationFactory<Program>
{
    private readonly string _redisConnectionString;
    private readonly string _instanceId;
    private readonly string _hangfirePrefix;
    private readonly string _statePrefix;

    public HangfireScaleOutWebApplicationFactory(
        string redisConnectionString,
        string instanceId,
        string hangfirePrefix,
        string statePrefix
    )
    {
        _redisConnectionString = redisConnectionString;
        _instanceId = instanceId;
        _hangfirePrefix = hangfirePrefix;
        _statePrefix = statePrefix;
    }

    protected override void ConfigureWebHost(
        Microsoft.AspNetCore.Hosting.IWebHostBuilder builder
    )
    {
        var settings = new Dictionary<string, string?>
        {
            ["ConnectionStrings:HangfireRedisConnection"] = _redisConnectionString,
            ["HangfireStorage:Provider"] = "Redis",
            ["HangfireStorage:SucceededJobExpirationInDays"] = "7",
            ["HangfireStorage:Redis:Database"] = "0",
            ["HangfireStorage:Redis:Prefix"] = _hangfirePrefix,
            ["HangfireServer:Enabled"] = "false",
            ["HangfireServer:BootstrapLockName"] = $"{_hangfirePrefix}bootstrap",
            ["HangfireServer:BootstrapLockTimeoutInSeconds"] = "30",
            ["TestService:InstanceId"] = _instanceId,
            ["TestService:StateDatabase"] = "1",
            ["TestService:StatePrefix"] = _statePrefix
        };

        builder.UseSetting(
            Microsoft.AspNetCore.Hosting.WebHostDefaults.EnvironmentKey,
            "Development"
        );
        foreach (var setting in settings)
        {
            builder.UseSetting(setting.Key, setting.Value);
        }

        builder.ConfigureAppConfiguration(
            (_, configurationBuilder) =>
            {
                configurationBuilder.AddInMemoryCollection(settings);
            }
        );
    }
}
