using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using SharedKernel.TickerQ.Abstractions;
using SharedKernel.TickerQ.Infrastructure;
using SharedKernel.TickerQ.Options;

namespace SharedKernel.TickerQ;

/// <summary>
/// Contains extension methods for configuring and integrating TickerQ with an application.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Configures TickerQ options and registers custom abstractions.
    /// </summary>
    /// <remarks>
    /// This method registers the SharedKernel abstractions for TickerQ.
    /// Consumers must also call the native TickerQ AddTickerQ() method separately
    /// to register the core TickerQ services.
    /// </remarks>
    /// <param name="services">The service collection to which the TickerQ configuration will be added.</param>
    /// <param name="configuration">The application's configuration that contains connection strings and settings.</param>
    /// <returns>The updated service collection with TickerQ services configured.</returns>
    public static IServiceCollection AddTickerQCustom(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var storageSection = configuration.GetSection(Constants.ConfigurationSections.TickerQStorage);
        var serverSection = configuration.GetSection(Constants.ConfigurationSections.TickerQServer);
        var dashboardSection = configuration.GetSection(Constants.ConfigurationSections.TickerQDashboard);

        // Bind configuration sections
        services.Configure<TickerQStorageOptions>(storageSection);
        services.Configure<TickerQServerOptions>(serverSection);
        services.Configure<TickerQDashboardOptions>(dashboardSection);

        // Register custom abstractions
        services.TryAddSingleton<ITickerQSchedulerEngine, TickerQSchedulerEngine>();
        services.TryAddScoped<IJobScheduler, JobSchedulerService>();

        return services;
    }

    /// <summary>
    /// Validates TickerQ configuration options.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection.</returns>
    public static IServiceCollection ValidateTickerQOptions(
        this IServiceCollection services
    )
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddOptions<TickerQStorageOptions>()
            .ValidateOnStart()
            .Validate(
                options =>
                {
                    if (string.IsNullOrWhiteSpace(options.Provider))
                    {
                        return false;
                    }

                    if (options.Provider.Equals(
                            TickerQStorageProviders.EntityFramework,
                            StringComparison.OrdinalIgnoreCase
                        ))
                    {
                        return !string.IsNullOrWhiteSpace(options.ConnectionStringName);
                    }

                    if (options.Provider.Equals(TickerQStorageProviders.Redis, StringComparison.OrdinalIgnoreCase))
                    {
                        return !string.IsNullOrWhiteSpace(options.Redis.ConnectionString);
                    }

                    return false;
                },
                "Invalid TickerQ storage configuration"
            );

        services.AddOptions<TickerQDashboardOptions>()
            .ValidateOnStart()
            .Validate(
                options =>
                {
                    if (!options.Enabled)
                    {
                        return true;
                    }

                    if (string.IsNullOrWhiteSpace(options.DashboardUrl))
                    {
                        return false;
                    }

                    if (options.EnableBasicAuth)
                    {
                        return !string.IsNullOrWhiteSpace(options.Username)
                            && !string.IsNullOrWhiteSpace(options.Password);
                    }

                    return true;
                },
                "Invalid TickerQ dashboard configuration"
            );

        return services;
    }

    /// <summary>
    /// Gets the connection string for TickerQ storage from configuration.
    /// </summary>
    /// <param name="configuration">The configuration instance.</param>
    /// <param name="storageOptions">The storage options.</param>
    /// <returns>The connection string.</returns>
    public static string GetTickerQConnectionString(
        this IConfiguration configuration,
        TickerQStorageOptions storageOptions
    )
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(storageOptions);

        if (storageOptions.Provider.Equals(TickerQStorageProviders.EntityFramework, StringComparison.OrdinalIgnoreCase))
        {
            var connectionStringName = storageOptions.ConnectionStringName ?? "TickerQConnection";
            var connectionString = configuration.GetConnectionString(connectionStringName);

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    string.Format(
                        Constants.ErrorMessages.MissingConnectionString,
                        connectionStringName,
                        "EntityFramework"
                    )
                );
            }

            return connectionString;
        }

        if (storageOptions.Provider.Equals(TickerQStorageProviders.Redis, StringComparison.OrdinalIgnoreCase))
        {
            var redisConnectionString =
                storageOptions.Redis.ConnectionString
                ?? configuration.GetConnectionString("TickerQRedisConnection")
                ?? configuration["Redis:UrlConfiguration"];

            if (string.IsNullOrWhiteSpace(redisConnectionString))
            {
                throw new InvalidOperationException(
                    string.Format(
                        Constants.ErrorMessages.MissingConnectionString,
                        "Redis",
                        "Redis"
                    )
                );
            }

            return redisConnectionString;
        }

        throw new InvalidOperationException(
            string.Format(
                Constants.ErrorMessages.UnsupportedStorageProvider,
                storageOptions.Provider
            )
        );
    }
}
