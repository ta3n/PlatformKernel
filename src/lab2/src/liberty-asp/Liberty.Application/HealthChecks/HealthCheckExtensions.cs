using HealthChecks.UI.Client;
using Liberty.Application.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Liberty.Application.HealthChecks;

public static class HealthCheckExtensions
{
    /// <summary>
    /// Add Custom HealthCheck
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddCustomHealthCheck(this IServiceCollection services, IConfiguration configuration)
    {
        var healthOptions = configuration.Get<HealthOptions>();
        var config = configuration.Get<AppSetting>();

        if (healthOptions != null && !healthOptions.Enabled) return services;

        var healthChecksBuilder = services.AddHealthChecks();

        if (config?.ConnectionString is not null)
            healthChecksBuilder.AddMySql(config.ConnectionString.DataContextConnection ?? string.Empty);

        services.AddHealthChecksUI(setup =>
        {
            setup.SetEvaluationTimeInSeconds(60);
            setup.AddHealthCheckEndpoint($"Liberty Health Check - {config?.App.AppName}", "/healthz");
        }).AddInMemoryStorage();

        return services;
    }

    /// <summary>
    /// Custom HealthCheck
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static WebApplication UseCustomHealthCheck(this WebApplication app)
    {
        var healthOptions = app.Configuration.Get<HealthOptions>();

        if (healthOptions != null && !healthOptions.Enabled) return app;

        app.UseHealthChecks("/healthz",
                new HealthCheckOptions
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
                    ResultStatusCodes =
                    {
                        [HealthStatus.Healthy] = StatusCodes.Status200OK,
                        [HealthStatus.Degraded] = StatusCodes.Status500InternalServerError,
                        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                    }
                })
            .UseHealthChecksUI(options =>
                {
                    options.ApiPath = "/healthcheck";
                    options.UIPath = "/healthcheck-ui";
                });

        return app;
    }
}
