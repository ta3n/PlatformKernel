using System.IO.Compression;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ServiceDiscovery;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace PlatformKernel.ServiceDefaults;

// Adds common .NET Aspire services: service discovery, resilience, health checks, and OpenTelemetry.
// This project should be referenced by each service project in your solution.
// To learn more about using this project, see https://aka.ms/dotnet/aspire/service-defaults
/// <summary>
/// Provides extension methods for configuring service discovery, resilience, health checks,
/// OpenTelemetry, response compression, and default endpoints in ASP.NET Core applications.
/// </summary>
public static class Extensions
{
    private const string HealthEndpointPath = "/health";
    private const string AlivenessEndpointPath = "/alive";
    private const string HealthChecksPolicy = "HealthChecks";

    /// <summary>
    /// Configures the application with a set of common .NET Aspire service defaults,
    /// including service discovery, resilience, health checks, and OpenTelemetry.
    /// These defaults simplify the setup of typical service capabilities for a .NET application.
    /// </summary>
    /// <param name="builder">The <see cref="IHostApplicationBuilder"/> used to configure the application.</param>
    /// <returns>The updated <see cref="IHostApplicationBuilder"/> with the service defaults applied.</returns>
    public static IHostApplicationBuilder AddServiceDefaults(
        this IHostApplicationBuilder builder
    )
    {
        builder.ConfigureOpenTelemetry();

        builder.AddDefaultHealthChecks();

        builder.Services.AddServiceDiscovery();

        builder.Services.ConfigureHttpClientDefaults(
            http =>
            {
                // Turn on resilience by default
                http.AddStandardResilienceHandler();

                // Turn on service discovery by default
                http.AddServiceDiscovery();
            }
        );

        // Uncomment the following to restrict the allowed schemes for service discovery.
        builder.Services.Configure<ServiceDiscoveryOptions>(
            options =>
            {
                options.AllowedSchemes = ["https", "http"];
            }
        );

        LogRuntimeInfo.Log();

        return builder;
    }

    /// <summary>
    /// Configures OpenTelemetry for tracing, metrics, and logging in the application.
    /// This includes setting up default instrumentation and exporters.
    /// </summary>
    /// <param name="builder">The <see cref="IHostApplicationBuilder"/> used to configure the application.</param>
    /// <returns>The configured <see cref="IHostApplicationBuilder"/> instance for further customization.</returns>
    private static IHostApplicationBuilder ConfigureOpenTelemetry(
        this IHostApplicationBuilder builder
    )
    {
        builder.Logging.AddOpenTelemetry(
            logging =>
            {
                logging.IncludeFormattedMessage = true;
                logging.IncludeScopes = true;
            }
        );

        builder.Services
            .AddOpenTelemetry()
            .WithMetrics(
                metrics =>
                {
                    metrics
                        .AddMeter("System.Runtime")
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddRuntimeInstrumentation();
                }
            )
            .WithTracing(
                tracing =>
                {
                    tracing
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation();
                }
            );

        builder.AddOpenTelemetryExporters();

        return builder;
    }

    /// Configures and enables the necessary OpenTelemetry exporters for the application.
    /// Determines if the OTLP exporter should be used based on the configuration settings
    /// and sets it up accordingly.
    /// <param name="builder">The host application builder to configure the OpenTelemetry exporters for.</param>
    /// <returns>The same <see cref="IHostApplicationBuilder"/> instance for method chaining.</returns>
    private static IHostApplicationBuilder AddOpenTelemetryExporters(
        this IHostApplicationBuilder builder
    )
    {
        var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

        if (useOtlpExporter)
        {
            builder.Services.AddOpenTelemetry().UseOtlpExporter();
        }

        return builder;
    }

    /// <summary>
    /// Adds default health checks to the application's dependency injection container.
    /// This includes a liveness check to verify the application's responsiveness.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="IHostApplicationBuilder"/> used to configure application services.
    /// </param>
    /// <returns>
    /// The <see cref="IHostApplicationBuilder"/> with default health checks added.
    /// </returns>
    public static IHostApplicationBuilder AddDefaultHealthChecks(
        this IHostApplicationBuilder builder
    )
    {
        var healthChecksConfiguration = builder.Configuration.GetSection(HealthChecksPolicy);

        // All health checks endpoints must return within the configured timeout value (defaults to 5 seconds)
        var healthChecksRequestTimeout = healthChecksConfiguration.GetValue<TimeSpan?>("RequestTimeout") ?? TimeSpan.FromSeconds(5);
        builder.Services.AddRequestTimeouts(timeouts => timeouts.AddPolicy(HealthChecksPolicy, healthChecksRequestTimeout));

        // Cache health checks responses for the configured duration (defaults to 10 seconds)
        var healthChecksExpireAfter = healthChecksConfiguration.GetValue<TimeSpan?>("ExpireAfter") ?? TimeSpan.FromSeconds(10);
        builder.Services.AddOutputCache(caching => caching.AddPolicy(HealthChecksPolicy, policy => policy.Expire(healthChecksExpireAfter)));

        builder.Services.AddHealthChecks()
            // Add a default liveness check to ensure app is responsive
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

        return builder;
    }

    /// <summary>
    /// Maps the default endpoints, including health checks and system version information, to the specified web application.
    /// </summary>
    /// <param name="app">The instance of <see cref="WebApplication"/> to which default endpoints will be mapped.</param>
    /// <returns>The same instance of <see cref="WebApplication"/> for chaining additional application configuration.</returns>
    public static WebApplication MapDefaultEndpoints(
        this WebApplication app
    )
    {
        // Adding health checks endpoints to applications in non-development environments has security implications.
        // See https://aka.ms/dotnet/aspire/healthchecks for details before enabling these endpoints in non-development environments.
        if (!app.Environment.IsDevelopment() && !app.Environment.IsStaging() && !app.Environment.IsProduction())
        {
            return app;
        }

        var healthChecks = app.MapGroup("");

        // Configure health checks endpoints to use the configured request timeouts and cache policies
        healthChecks
            .CacheOutput(HealthChecksPolicy)
            .WithRequestTimeout(HealthChecksPolicy);

        // All health checks must pass for app to be considered ready to accept traffic after starting
        healthChecks.MapHealthChecks(HealthEndpointPath);

        // Only health checks tagged with the "live" tag must pass for app to be considered alive
        healthChecks.MapHealthChecks(AlivenessEndpointPath, new() { Predicate = r => r.Tags.Contains("live") });

        // Add the health checks endpoint for the HealthChecksUI
        var healthChecksUrls = app.Configuration["HEALTHCHECKSUI_URLS"];
        if (!string.IsNullOrWhiteSpace(healthChecksUrls))
        {
            var pathToHostsMap = GetPathToHostsMap(healthChecksUrls);

            foreach (var path in pathToHostsMap.Keys)
            {
                // Ensure that the HealthChecksUI endpoint is only accessible from configured hosts, e.g. localhost:12345, hub.docker.internal, etc.
                // as it contains more detailed information about the health of the app including the types of dependencies it has.

                healthChecks.MapHealthChecks(path, new() { ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse })
                    // This ensures that the HealthChecksUI endpoint is only accessible from the configured health checks URLs.
                    // See this documentation to learn more about restricting access to health checks endpoints via routing:
                    // https://learn.microsoft.com/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-8.0#use-health-checks-routing
                    .RequireHost(pathToHostsMap[path]);
            }
        }

        if (app.Environment.IsDevelopment())
        {
            app
                .MapGet("/runtime-info", LogRuntimeInfo.GetRuntimeInfoAsText)
                .WithName("GetRuntimeInfo")
                .WithTags("System")
                .Produces(200)
                .Produces(404)
                .ExcludeFromDescription();
        }

        //Get Name And Version
        app
            .MapGet(
                "api/version",
                (
                    IConfiguration config
                ) =>
                {
                    IResult result;

                    var appName = config["App:appName"];
                    var appVersion = config["App:AppVersion"];

                    if (string.IsNullOrEmpty(appVersion))
                    {
                        result = TypedResults.NotFound("Version info not found");
                    }
                    else
                    {
                        result = TypedResults.Ok(
                            new
                            {
                                appName,
                                appVersion
                            }
                        );
                    }

                    return result;
                }
            )
            .WithName("GetVersion")
            .WithTags("System")
            .Produces(200)
            .Produces(404);

        return app;
    }

    /// <summary>
    /// Enables response compression by configuring the services in the application builder. This method sets up Gzip compression,
    /// supports HTTPS, and adds specific MIME types to be compressed.
    /// </summary>
    /// <param name="builder">The <see cref="IHostApplicationBuilder"/> instance to configure.</param>
    /// <returns>The configured <see cref="IHostApplicationBuilder"/> instance.</returns>
    public static IHostApplicationBuilder EnableResponseCompression(
        this IHostApplicationBuilder builder
    )
    {
        builder.Services.AddResponseCompression(
            options =>
            {
                options.EnableForHttps = true; // Support HTTPS
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>(); // Use Gzip to reduce CPU load
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    [
                        "application/json",
                        "text/html",
                        "text/plain"
                    ]
                );
            }
        );

        builder.Services.Configure<BrotliCompressionProviderOptions>(
            opts =>
            {
                opts.Level = CompressionLevel.Fastest;
            }
        );

        builder.Services.Configure<GzipCompressionProviderOptions>(
            options =>
            {
                options.Level = CompressionLevel.Fastest; // Reduce CPU load
            }
        );

        return builder;
    }

    /// <summary>
    /// Configures the application to use default response compression settings.
    /// </summary>
    /// <param name="app">The <see cref="WebApplication"/> instance to configure.</param>
    /// <returns>The <see cref="WebApplication"/> instance after configuring response compression.</returns>
    public static WebApplication UseResponseCompressionDefaults(
        this WebApplication app
    )
    {
        // Enable Response Compression Middleware
        app.UseResponseCompression();

        return app;
    }

    private static Dictionary<string, string[]> GetPathToHostsMap(
        string healthChecksUrls
    )
    {
        // Given a value like "localhost:12345/healthz;hub.docker.internal:12345/healthz" return a dictionary like:
        // { { "healthz", [ "localhost:12345", "hub.docker.internal:12345" ] } }

        var uris = healthChecksUrls.Split(';', StringSplitOptions.RemoveEmptyEntries)
            .Select(url => new Uri(url, UriKind.Absolute))
            .GroupBy(uri => uri.AbsolutePath, uri => uri.Authority)
            .ToDictionary(g => g.Key, g => g.ToArray());

        return uris;
    }
}
