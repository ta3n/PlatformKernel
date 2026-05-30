using Hangfire;
using Hangfire.PostgreSql;
using Hangfire.Redis.StackExchange;
using HangfireBasicAuthenticationFilter;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using SharedKernel.Hangfire.Abstractions;
using SharedKernel.Hangfire.Infrastructure;
using SharedKernel.Hangfire.Options;
using SharedKernel.Hangfire.Utils;

namespace SharedKernel.Hangfire;

/// <summary>
/// Contains extension methods for configuring and integrating Hangfire with an application.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Configures Hangfire services with PostgreSQL storage and custom retry mechanisms.
    /// </summary>
    /// <param name="services">The service collection to which the Hangfire configuration will be added.</param>
    /// <param name="configuration">The application's configuration that contains the connection string for PostgreSQL storage and other settings.</param>
    /// <returns>The updated service collection with Hangfire services configured.</returns>
    public static IServiceCollection AddHangfireCustom(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var hangfireStorageSection = configuration.GetSection("HangfireStorage");
        var hangfireServerSection = configuration.GetSection("HangfireServer");
        var hangfireStorageOptions =
            hangfireStorageSection.Get<HangfireStorageOptions>() ?? new HangfireStorageOptions();
        var hangfireServerOptions = hangfireServerSection.Get<HangfireServerOptions>() ?? new HangfireServerOptions();
        var succeededJobExpirationTimeout = GetSucceededJobExpirationTimeout(
            hangfireStorageOptions.SucceededJobExpirationInDays
        );

        services.Configure<HangfireStorageOptions>(hangfireStorageSection);
        services.Configure<HangfireServerOptions>(hangfireServerSection);

        services.AddHangfire(
            (
                serviceProvider,
                globalConfiguration
            ) =>
            {
                var loggerFactory = serviceProvider.GetService<ILoggerFactory>()
                    ?? NullLoggerFactory.Instance;

                globalConfiguration
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseDefaultTypeSerializer()
                    .ConfigureStorage(
                        configuration,
                        hangfireStorageOptions
                    )
                    .UseFilter(
                        new SchedulerRetryFilterAttribute()
                    )
                    .UseFilter(
                        new AutomaticRetryAttribute
                        {
                            Attempts = 3,
                            DelaysInSeconds = [60, 120, 180]
                        }
                    )
                    .UseFilter(
                        new CustomRetryFilterAttribute(
                            loggerFactory.CreateLogger<CustomRetryFilterAttribute>()
                        )
                    )
                    .UseFilter(
                        new SucceededJobExpirationFilterAttribute(
                            succeededJobExpirationTimeout
                        )
                    );
            }
        );

        if (hangfireServerOptions.Enabled)
        {
            services.AddHangfireServer();
        }

        services.TryAddSingleton<IHangfireSchedulerEngine, HangfireSchedulerEngine>();

        return services;
    }

    /// <summary>
    /// Registers scheduler orchestration services used by a dedicated Scheduler Service.
    /// </summary>
    /// <remarks>
    /// Consumers must also register <see cref="IJobMetadataStore"/> and <see cref="IGrpcDispatcher"/>.
    /// This method is kept separate from <see cref="AddHangfireCustom"/> so services that only host
    /// Hangfire storage/server infrastructure do not need scheduler-specific dependencies.
    /// </remarks>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddHangfireSchedulerOrchestration(
        this IServiceCollection services
    )
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddScoped<IJobScheduler, JobSchedulerService>();
        services.TryAddScoped<IJobExecutor, JobExecutor>();

        return services;
    }

    /// <summary>
    /// Registers the generic scheduler job executor used by a dedicated Scheduler Service.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddHangfireSchedulerExecutor(
        this IServiceCollection services
    )
    {
        return services.AddHangfireSchedulerOrchestration();
    }

    /// <summary>
    /// Configures and applies the Hangfire dashboard middleware to the application pipeline
    /// based on the provided configuration.
    /// </summary>
    /// <param name="app">
    /// The application builder instance to which the middleware is added.
    /// </param>
    /// <param name="configuration">
    /// The application's configuration, used to retrieve Hangfire dashboard settings.
    /// </param>
    /// <returns>
    /// The updated application builder instance with the Hangfire dashboard middleware configured.
    /// </returns>
    public static IApplicationBuilder UseHangfireDashboardCustom(
        this IApplicationBuilder app,
        IConfiguration configuration
    )
    {
        var section = configuration.GetSection("HangfireDashboard");
        var hangfireDashboardOptions = section.Get<HangfireDashboardOptions>();

        if (!(hangfireDashboardOptions?.Enabled ?? false))
        {
            return app;
        }

        ValidateDashboardOptions(hangfireDashboardOptions);

        app.UseHangfireDashboard(
            $"/{hangfireDashboardOptions.DashboardUrl}",
            new DashboardOptions
            {
                IsReadOnlyFunc = _ => hangfireDashboardOptions.IsReadOnly,
                Authorization =
                [
                    new HangfireCustomBasicAuthenticationFilter
                    {
                        User = hangfireDashboardOptions.Username,
                        Pass = hangfireDashboardOptions.Password
                    }
                ]
            }
        );

        return app;
    }

    private static void ValidateDashboardOptions(
        HangfireDashboardOptions options
    )
    {
        if (string.IsNullOrWhiteSpace(options.DashboardUrl))
        {
            throw new InvalidOperationException(
                "HangfireDashboard:DashboardUrl must be configured when Hangfire dashboard is enabled."
            );
        }

        if (string.IsNullOrWhiteSpace(options.Username) || string.IsNullOrWhiteSpace(options.Password))
        {
            throw new InvalidOperationException(
                "HangfireDashboard:Username and HangfireDashboard:Password must be configured when Hangfire dashboard is enabled."
            );
        }
    }

    /// <summary>
    /// Executes Hangfire bootstrap logic under a distributed lock so only one scaled instance
    /// can register recurring or scheduled jobs at a time.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <param name="configureJobs">The bootstrap action that registers Hangfire jobs.</param>
    /// <returns>The same application builder.</returns>
    public static IApplicationBuilder UseHangfireBootstrapLock(
        this IApplicationBuilder app,
        Action<IServiceProvider> configureJobs
    )
    {
        ArgumentNullException.ThrowIfNull(app);

        app.ApplicationServices.UseHangfireBootstrapLock(configureJobs);

        return app;
    }

    /// <summary>
    /// Executes Hangfire bootstrap logic under a distributed lock so only one scaled instance
    /// can register recurring or scheduled jobs at a time.
    /// </summary>
    /// <param name="serviceProvider">The root service provider.</param>
    /// <param name="configureJobs">The bootstrap action that registers Hangfire jobs.</param>
    /// <returns>The same root service provider.</returns>
    public static IServiceProvider UseHangfireBootstrapLock(
        this IServiceProvider serviceProvider,
        Action<IServiceProvider> configureJobs
    )
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(configureJobs);

        using var scope = serviceProvider.CreateScope();
        var scopedProvider = scope.ServiceProvider;
        var jobStorage = scopedProvider.GetRequiredService<JobStorage>();
        var hangfireServerOptions = scopedProvider
                .GetService<IOptions<HangfireServerOptions>>()
                ?
                .Value
            ?? new HangfireServerOptions();

        using var connection = jobStorage.GetConnection();
        using var distributedLock = connection.AcquireDistributedLock(
            GetBootstrapLockName(hangfireServerOptions.BootstrapLockName),
            GetBootstrapLockTimeout(hangfireServerOptions.BootstrapLockTimeoutInSeconds)
        );

        configureJobs(scopedProvider);

        return serviceProvider;
    }

    /// <summary>
    /// Executes Hangfire bootstrap logic under a distributed lock so only one scaled instance
    /// can register recurring or scheduled jobs at a time.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <param name="configureJobsAsync">The asynchronous bootstrap action that registers Hangfire jobs.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The same application builder.</returns>
    public static async Task<IApplicationBuilder> UseHangfireBootstrapLockAsync(
        this IApplicationBuilder app,
        Func<IServiceProvider, Task> configureJobsAsync,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(app);

        await app.ApplicationServices.UseHangfireBootstrapLockAsync(
            configureJobsAsync,
            cancellationToken
        );

        return app;
    }

    /// <summary>
    /// Executes Hangfire bootstrap logic under a distributed lock so only one scaled instance
    /// can register recurring or scheduled jobs at a time.
    /// </summary>
    /// <param name="serviceProvider">The root service provider.</param>
    /// <param name="configureJobsAsync">The asynchronous bootstrap action that registers Hangfire jobs.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The same root service provider.</returns>
    public static async Task<IServiceProvider> UseHangfireBootstrapLockAsync(
        this IServiceProvider serviceProvider,
        Func<IServiceProvider, Task> configureJobsAsync,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(configureJobsAsync);
        cancellationToken.ThrowIfCancellationRequested();

        using var scope = serviceProvider.CreateScope();
        var scopedProvider = scope.ServiceProvider;
        var jobStorage = scopedProvider.GetRequiredService<JobStorage>();
        var hangfireServerOptions = scopedProvider
                .GetService<IOptions<HangfireServerOptions>>()
                ?
                .Value
            ?? new HangfireServerOptions();

        using var connection = jobStorage.GetConnection();
        using var distributedLock = connection.AcquireDistributedLock(
            GetBootstrapLockName(hangfireServerOptions.BootstrapLockName),
            GetBootstrapLockTimeout(hangfireServerOptions.BootstrapLockTimeoutInSeconds)
        );

        await configureJobsAsync(scopedProvider);

        return serviceProvider;
    }

    private static IGlobalConfiguration ConfigureStorage(
        this IGlobalConfiguration globalConfiguration,
        IConfiguration configuration,
        HangfireStorageOptions hangfireStorageOptions
    )
    {
        return hangfireStorageOptions.Provider switch
        {
            var provider when provider.Equals(
                HangfireStorageProviders.Redis,
                StringComparison.OrdinalIgnoreCase
            ) => globalConfiguration.ConfigureRedisStorage(
                configuration,
                hangfireStorageOptions.Redis
            ),
            var provider when provider.Equals(
                HangfireStorageProviders.PostgreSql,
                StringComparison.OrdinalIgnoreCase
            ) => globalConfiguration.ConfigurePostgreSqlStorage(
                configuration
            ),
            _ => throw new InvalidOperationException(
                $"Unsupported Hangfire storage provider '{hangfireStorageOptions.Provider}'. Supported values are '{HangfireStorageProviders.PostgreSql}' and '{HangfireStorageProviders.Redis}'."
            )
        };
    }

    private static IGlobalConfiguration ConfigurePostgreSqlStorage(
        this IGlobalConfiguration globalConfiguration,
        IConfiguration configuration
    )
    {
        var dbConnectionString = configuration.GetConnectionString("HangfireConnection");

        if (string.IsNullOrWhiteSpace(dbConnectionString))
        {
            throw new InvalidOperationException(
                "Missing connection string 'HangfireConnection' for Hangfire PostgreSQL storage."
            );
        }

        return globalConfiguration.UsePostgreSqlStorage(
            options =>
                options.UseNpgsqlConnection(dbConnectionString)
        );
    }

    private static IGlobalConfiguration ConfigureRedisStorage(
        this IGlobalConfiguration globalConfiguration,
        IConfiguration configuration,
        HangfireRedisStorageOptions redisStorageOptions
    )
    {
        var redisConnectionString =
            redisStorageOptions.ConnectionString
            ?? configuration.GetConnectionString("HangfireRedisConnection")
            ?? configuration["Redis:UrlConfiguration"];

        if (string.IsNullOrWhiteSpace(redisConnectionString))
        {
            throw new InvalidOperationException(
                "Missing Redis configuration for Hangfire. Configure 'HangfireStorage:Redis:ConnectionString', 'ConnectionStrings:HangfireRedisConnection', or 'Redis:UrlConfiguration'."
            );
        }

        var options = new RedisStorageOptions();

        if (!redisStorageOptions.Database.HasValue)
        {
            if (configuration.GetValue<int?>("Redis:DefaultDatabase") is { } defaultDatabase)
            {
                options.Db = ValidateNonNegative(
                    defaultDatabase,
                    "Redis:DefaultDatabase"
                );
            }
        }
        else
        {
            options.Db = ValidateNonNegative(
                redisStorageOptions.Database.Value,
                "HangfireStorage:Redis:Database"
            );
        }

        if (!string.IsNullOrWhiteSpace(redisStorageOptions.Prefix))
        {
            options.Prefix = redisStorageOptions.Prefix;
        }

        if (redisStorageOptions.SucceededListSize.HasValue)
        {
            options.SucceededListSize = ValidateNonNegative(
                redisStorageOptions.SucceededListSize.Value,
                "HangfireStorage:Redis:SucceededListSize"
            );
        }

        if (redisStorageOptions.DeletedListSize.HasValue)
        {
            options.DeletedListSize = ValidateNonNegative(
                redisStorageOptions.DeletedListSize.Value,
                "HangfireStorage:Redis:DeletedListSize"
            );
        }

        if (redisStorageOptions.ExpiryCheckIntervalInSeconds.HasValue)
        {
            options.ExpiryCheckInterval = ToPositiveTimeSpan(
                redisStorageOptions.ExpiryCheckIntervalInSeconds.Value,
                "HangfireStorage:Redis:ExpiryCheckIntervalInSeconds"
            );
        }

        if (redisStorageOptions.FetchTimeoutInSeconds.HasValue)
        {
            options.FetchTimeout = ToPositiveTimeSpan(
                redisStorageOptions.FetchTimeoutInSeconds.Value,
                "HangfireStorage:Redis:FetchTimeoutInSeconds"
            );
        }

        if (redisStorageOptions.InvisibilityTimeoutInSeconds.HasValue)
        {
            options.InvisibilityTimeout = ToPositiveTimeSpan(
                redisStorageOptions.InvisibilityTimeoutInSeconds.Value,
                "HangfireStorage:Redis:InvisibilityTimeoutInSeconds"
            );
        }

        if (redisStorageOptions.UseTransactions.HasValue)
        {
            options.UseTransactions = redisStorageOptions.UseTransactions.Value;
        }

        return globalConfiguration.UseRedisStorage(
            redisConnectionString,
            options
        );
    }

    private static TimeSpan GetSucceededJobExpirationTimeout(
        int succeededJobExpirationInDays
    )
    {
        return succeededJobExpirationInDays > 0
            ? TimeSpan.FromDays(succeededJobExpirationInDays)
            : throw new ArgumentOutOfRangeException(
                nameof(succeededJobExpirationInDays),
                "HangfireStorage:SucceededJobExpirationInDays must be greater than zero."
            );
    }

    private static string GetBootstrapLockName(
        string bootstrapLockName
    )
    {
        return !string.IsNullOrWhiteSpace(bootstrapLockName)
            ? bootstrapLockName
            : throw new ArgumentException(
                "Hangfire bootstrap lock name must not be empty.",
                nameof(bootstrapLockName)
            );
    }

    private static TimeSpan GetBootstrapLockTimeout(
        int bootstrapLockTimeoutInSeconds
    )
    {
        return bootstrapLockTimeoutInSeconds > 0
            ? TimeSpan.FromSeconds(bootstrapLockTimeoutInSeconds)
            : throw new ArgumentOutOfRangeException(
                nameof(bootstrapLockTimeoutInSeconds),
                "HangfireServer:BootstrapLockTimeoutInSeconds must be greater than zero."
            );
    }

    private static int ValidateNonNegative(
        int value,
        string settingName
    )
    {
        return value >= 0
            ? value
            : throw new ArgumentOutOfRangeException(
                settingName,
                $"{settingName} must be greater than or equal to zero."
            );
    }

    private static TimeSpan ToPositiveTimeSpan(
        int value,
        string settingName
    )
    {
        return value > 0
            ? TimeSpan.FromSeconds(value)
            : throw new ArgumentOutOfRangeException(
                settingName,
                $"{settingName} must be greater than zero."
            );
    }
}
