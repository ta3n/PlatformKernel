using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace SharedKernel.Hangfire.Test.Service.Services;

internal static class RedisConnectionFactory
{
    public static IConnectionMultiplexer Create(
        IConfiguration configuration
    )
    {
        var connectionString =
            configuration.GetConnectionString("HangfireRedisConnection")
            ?? configuration["Redis:UrlConfiguration"];

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Missing Redis connection string for Hangfire test service."
            );
        }

        var configurationOptions = ConfigurationOptions.Parse(connectionString);
        configurationOptions.AllowAdmin = true;
        configurationOptions.AbortOnConnectFail = false;

        return ConnectionMultiplexer.Connect(configurationOptions);
    }
}
