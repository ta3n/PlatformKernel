using Hangfire;
using Hangfire.PostgreSql;
using HangfireBasicAuthenticationFilter;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
        var dbConnectionString = configuration.GetConnectionString("HangfireConnection");

        services.AddHangfire(
            globalConfiguration =>
            {
                globalConfiguration
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseDefaultTypeSerializer()
                    .UsePostgreSqlStorage(
                        options =>
                            options.UseNpgsqlConnection(dbConnectionString)
                    )
                    .UseFilter(
                        new AutomaticRetryAttribute
                        {
                            Attempts = 3, // Number of retry attempts
                            DelaysInSeconds = [60, 120, 180] // Delay between retries in seconds
                        }
                    )
                    .UseFilter(
                        new CustomRetryFilterAttribute(
                            new LoggerFactory().CreateLogger<CustomRetryFilterAttribute>()
                        )
                    );
            }
        );

        services.AddHangfireServer();

        return services;
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
}
