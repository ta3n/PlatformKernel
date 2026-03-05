using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sentry;
using Sentry.Extensions.Logging;
using SentryOptions = Liberty.SentrySelfHosted.Options.SentryOptions;

namespace Liberty.SentrySelfHosted;

/// <summary>
/// Provides extension methods for configuring and using Sentry monitoring in an ASP.NET Core application.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Configures and enables Sentry monitoring for the application.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="WebApplicationBuilder"/> used to configure the web application.
    /// </param>
    /// <param name="beforeSend">
    /// An optional function to process or filter Sentry events before they are sent.
    /// This function receives the <see cref="SentryEvent"/> and an associated <see cref="Hint"/>.
    /// If the function returns null, the event is discarded.
    /// </param>
    /// <param name="logEntryFilter">
    /// An optional function to filter log entries for Sentry.
    /// The function receives the log category, log level, <see cref="EventId"/>, and an optional exception.
    /// If the function returns false, the log entry will be ignored for Sentry.
    /// </param>
    /// <returns>
    /// The updated <see cref="IHostApplicationBuilder"/> instance with Sentry monitoring configured and enabled.
    /// </returns>
    public static IHostApplicationBuilder AddSentryMonitoring(
        this WebApplicationBuilder builder,
        Func<SentryEvent, Hint, SentryEvent?>? beforeSend = null,
        Func<string, LogLevel, EventId, Exception?, bool>? logEntryFilter = null
    )
    {
        var configuration = builder.Configuration;
        var section = configuration.GetSection("SentryMonitoring");
        var sentryOptions = section.Get<SentryOptions>();

        builder.Services.Configure<SentryOptions>(section);

        if (sentryOptions is null || !sentryOptions.Enabled)
        {
            return builder;
        }

        // SentrySdk.Init(
        //     options =>
        //     {
        //         options.Dsn = sentryOptions.Dsn;
        //         options.TracesSampleRate = sentryOptions.TracesSampleRate;
        //         options.SendClientReports = false;
        //         // Add the EntityFramework integration
        //         options.AddEntityFramework();
        //
        //         // options.UseOpenTelemetry(); // <-- Configure Sentry to use OpenTelemetry trace information
        //     }
        // );

        builder.WebHost.UseSentry(
            options =>
            {
                options.MaxRequestBodySize = Sentry.Extensibility.RequestSize.Always;

                options.Dsn = sentryOptions.Dsn;
                // When configuring for the first time, to see what the SDK is doing:
                options.Debug = sentryOptions.Debug;
                // Set TracesSampleRate to 1.0 to capture 100%
                // of transactions for performance monitoring.
                // We recommend adjusting this value in production
                options.TracesSampleRate = sentryOptions.TracesSampleRate;
                options.SendClientReports = false;
                // Add the EntityFramework integration
                options.AddEntityFramework();

                options.Environment = builder.Environment.EnvironmentName;

                // options.UseOpenTelemetry(); // <-- Configure Sentry to use OpenTelemetry trace information

                if (logEntryFilter is not null)
                {
                    options.AddLogEntryFilter(logEntryFilter);
                }

                if (beforeSend is not null)
                {
                    options.SetBeforeSend(beforeSend);
                }
            }
        );

        builder.Services.AddSentry();
        builder.Services.AddSentryTunneling();

        return builder;
    }

    /// <summary>
    /// Configures the application to use Sentry monitoring if enabled in the configuration.
    /// Adds Sentry tracing and tunneling middleware to the application pipeline when applicable.
    /// </summary>
    /// <param name="builder">The application builder instance used to configure the application's request pipeline.</param>
    /// <param name="configuration">The configuration instance to retrieve Sentry monitoring settings.</param>
    /// <returns>The updated application builder instance.</returns>
    public static IApplicationBuilder UseSentryMonitoring(
        this IApplicationBuilder builder,
        IConfiguration configuration
    )
    {
        var section = configuration.GetSection("SentryMonitoring");
        var sentryOptions = section.Get<SentryOptions>();

        if (sentryOptions is null)
        {
            return builder;
        }

        if (!sentryOptions.Enabled)
        {
            return builder;
        }

        builder.UseSentryTracing();
        builder.UseSentryTunneling();

        return builder;
    }
}
