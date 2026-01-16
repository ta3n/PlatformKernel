using System.Collections.Concurrent;
using System.Diagnostics;
using Amazon.CloudWatchLogs;
using Amazon.Runtime;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlatformKernel.Serilog.HostedServices;
using PlatformKernel.Serilog.Options;
using Sentry;
using Serilog;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;
using Serilog.Formatting.Display;
using Serilog.Formatting.Json;
using Serilog.Sinks.AwsCloudWatch;
using Serilog.Sinks.SystemConsole.Themes;

namespace PlatformKernel.Serilog;

using SentryOptions = Options.SentryOptions;

/// <summary>
/// Provides extension methods for configuring Serilog logging in an ASP.NET application.
/// This class includes methods for adding and configuring Serilog logging
/// during application startup and runtime.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Configures Serilog logging for the application, enabling structured logging
    /// based on the specified configuration and application name.
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder instance used to configure the application.</param>
    /// <param name="applicationName">The name of the application, used in Serilog log configuration.</param>
    /// <returns>The modified <see cref="IHostApplicationBuilder"/> instance with Serilog logging configured.</returns>
    public static IHostApplicationBuilder AddSerilogLogging(
        this WebApplicationBuilder builder,
        string applicationName
    )
    {
        var configuration = builder.Configuration;
        var section = configuration.GetSection("SerilogLogging");
        var serilogOptions = section.Get<SerilogLoggingOptions>();

        builder.Services.Configure<SerilogLoggingOptions>(section);

        if (serilogOptions?.Enabled is not true)
        {
            return builder;
        }

        Log.Logger = new LoggerConfiguration()
            .SetupLoggerConfiguration(
                builder,
                serilogOptions,
                applicationName
            )
            .CreateLogger();

        builder.Host.UseSerilog(
            (
                _,
                loggerConfig
            ) =>
            {
                loggerConfig.SetupLoggerConfiguration(
                    builder,
                    serilogOptions,
                    applicationName
                );
            }
        );

        SelfLog.Enable(
            msg =>
            {
                Debug.Print(msg);
            }
        );

        ConfigureLogProcessorService(builder.Services);

        return builder;
    }

    /// <summary>
    /// Configures the log processor service by adding required dependencies to the service collection.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> where the dependencies for the log processor service
    /// are registered, such as the log queue and the <see cref="LogProcessorService"/>.
    /// </param>
    private static void ConfigureLogProcessorService(
        IServiceCollection services
    )
    {
        var logQueue = new ConcurrentQueue<string>();
        services.AddSingleton(logQueue);
        services.AddHostedService<LogProcessorService>();
    }

    /// <summary>
    /// Configures the application to use Serilog request logging middleware, which logs HTTP requests and responses.
    /// </summary>
    /// <param name="builder">An instance of <see cref="IApplicationBuilder"/> used to configure the application's request pipeline.</param>
    /// <returns>The <see cref="IApplicationBuilder"/> instance for further configuration.</returns>
    public static IApplicationBuilder UseSerilogLogging(
        this IApplicationBuilder builder
    )
    {
        builder.UseSerilogRequestLogging();

        return builder;
    }

    /// Configures the Serilog logging pipeline with specified options and settings.
    /// <param name="loggerConfig">
    /// The current instance of the Serilog LoggerConfiguration to configure.
    /// </param>
    /// <param name="builder">
    /// The WebApplicationBuilder instance that contains application services and configuration used to set up logging.
    /// </param>
    /// <param name="serilogOptions">
    /// An instance of SerilogLoggingOptions containing the configuration for Serilog logging sinks and enrichers.
    /// </param>
    /// <param name="applicationName">
    /// The name of the application, which will be included in the log entries to identify their origin.
    /// </param>
    /// <returns>
    /// Returns the configured instance of LoggerConfiguration after applying the necessary settings.
    /// </returns>
    private static LoggerConfiguration SetupLoggerConfiguration(
        this LoggerConfiguration loggerConfig,
        WebApplicationBuilder builder,
        SerilogLoggingOptions serilogOptions,
        string applicationName
    )
    {
        var configuration = builder.Configuration;
        var environment = builder.Environment;

        loggerConfig
            .MinimumLevel.Information()
            .MinimumLevel.Override(
                "Microsoft",
                LogEventLevel.Information
            )
            .MinimumLevel.Override(
                "System",
                LogEventLevel.Warning
            )
            .MinimumLevel.Override(
                "Microsoft.AspNetCore",
                LogEventLevel.Warning
            )
            .MinimumLevel.Override(
                "Microsoft.AspNetCore.Hosting",
                LogEventLevel.Warning
            );

        loggerConfig.ReadFrom.Configuration(configuration);

        loggerConfig.Enrich
            .FromLogContext()
            .Enrich
            .WithMachineName()
            .Enrich
            .WithEnvironmentName()
            .Enrich
            .WithEnvironmentUserName()
            .Enrich
            .WithExceptionDetails(
                new DestructuringOptionsBuilder()
                    .WithDefaultDestructurers()
                    .WithDestructurers(
                        [
                            new DbUpdateExceptionDestructurer()
                        ]
                    )
            )
            .Enrich
            .WithProperty("MachineName", Environment.MachineName)
            .Enrich
            .WithProperty("ProcessId", Environment.ProcessId)
            .Enrich
            .WithProperty("ProcessName", Process.GetCurrentProcess().ProcessName)
            .Enrich
            .WithProperty("ThreadId", Environment.CurrentManagedThreadId)
            .Enrich
            .WithProperty("Environment", environment.EnvironmentName)
            .Enrich
            .WithProperty("ApplicationName", applicationName)
            .Enrich
            .With<LoggerNameEnricher>()
            .WriteToSeq(serilogOptions.WriteToSeq)
            .WriteToSentry(serilogOptions.WriteToSentry)
            .WriteToAwsCloudWatch(serilogOptions.WriteToAwsCloudWatch);

        if (serilogOptions.WriteToConsole)
        {
            loggerConfig.WriteTo.Async(
                configure => configure.Console(
                    outputTemplate:
                    "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u4} {ProcessId} --- [{ThreadId,3}] [{Properties:j}] {LoggerName} : {Message:lj}{NewLine}{Exception}",
                    theme: AnsiConsoleTheme.Code
                )
            );
        }

        if (serilogOptions.WriteToFile)
        {
            loggerConfig.WriteTo.Async(
                configure => configure.File(
                    new JsonFormatter(),
                    "_logs/app-logs-.txt",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    fileSizeLimitBytes: 52428800 // 50MB
                )
            );
        }

        var otlpEndpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");
        if (!string.IsNullOrEmpty(otlpEndpoint))
        {
            loggerConfig.WriteTo.Async(
                configure => configure.OpenTelemetry(
                    otlpEndpoint
                )
            );
        }

        return loggerConfig;
    }

    /// <summary>
    /// Configures the given <see cref="LoggerConfiguration"/> to write log events to a Seq server.
    /// </summary>
    /// <param name="loggerConfig">
    /// The <see cref="LoggerConfiguration"/> object to extend with Seq logging capabilities.
    /// </param>
    /// <param name="seqOptions">
    /// Configuration options for connecting to a Seq server, including the server URL,
    /// API key, and a flag indicating whether Seq logging is enabled.
    /// </param>
    /// <returns>
    /// The modified <see cref="LoggerConfiguration"/> instance, configured to write to Seq if enabled in the provided options.
    /// </returns>
    private static LoggerConfiguration WriteToSeq(
        this LoggerConfiguration loggerConfig,
        SeqOptions? seqOptions
    )
    {
        if (seqOptions is null || !seqOptions.Enabled)
        {
            return loggerConfig;
        }

        loggerConfig.WriteTo.Async(
            configure => configure.Seq(
                seqOptions.Url,
                apiKey: seqOptions.ApiKey
            )
        );

        return loggerConfig;
    }

    /// <summary>
    /// Configures the logger to write log events to AWS CloudWatch.
    /// </summary>
    /// <param name="loggerConfig">The logger configuration to extend.</param>
    /// <param name="awsCloudWatchOptions">Configuration options required for AWS CloudWatch, including credentials and settings.</param>
    /// <returns>The updated logger configuration with AWS CloudWatch settings applied.</returns>
    private static LoggerConfiguration WriteToAwsCloudWatch(
        this LoggerConfiguration loggerConfig,
        AwsCloudWatchOptions? awsCloudWatchOptions
    )
    {
        if (awsCloudWatchOptions is null || !awsCloudWatchOptions.Enabled)
        {
            return loggerConfig;
        }

        // Setup AWS CloudWatch client
        var client = new AmazonCloudWatchLogsClient(
            new BasicAWSCredentials(
                awsCloudWatchOptions.AwsSetting.AccessKeyId,
                awsCloudWatchOptions.AwsSetting.SecretAccessKey
            )
        );

        loggerConfig.WriteTo.Async(
            configure => configure.AmazonCloudWatch(
                // The name of the log group to log to
                awsCloudWatchOptions.LogGroupName,
                // A string that our log stream names should be prefixed with. We are just specifying the
                // start timestamp as the log stream prefix
                $"{awsCloudWatchOptions.LogStreamPrefix}-{DateTime.UtcNow:yyyyMMddHHmmssfff}",
                LogEventLevel.Verbose,
                // (Optional) Maximum number of log events that should be sent in a batch to AWS CloudWatch
                99,
                // (Optional) The maximum number of log messages that are stored locally before being sent to AWS Cloudwatch
                queueSizeLimit: 10000,
                // (Optional) Similar to above, except the maximum amount of time that should pass before
                // log events must be sent to AWS CloudWatch
                batchUploadPeriodInSeconds: 15,
                // (Optional) If the log group does not exist, should we try to create it?
                createLogGroup: true,
                appendUniqueInstanceGuid: true,
                appendHostName: true,
                // (Optional) The number of attempts we should make when logging events that fail
                maxRetryAttempts: 5,
                // (Optional) Specify the time that logs should be kept for in AWS CloudWatch
                logGroupRetentionPolicy: LogGroupRetentionPolicy.OneMonth,
                // (Optional) Specify a custom text formatter for the output message.
                textFormatter: new JsonFormatter(),
                // The AWS CloudWatch client to use
                cloudWatchClient: client
            )
        );

        return loggerConfig;
    }

    /// <summary>
    /// Configures the logger to send log events to Sentry based on the provided Sentry options.
    /// </summary>
    /// <param name="loggerConfig">The <see cref="LoggerConfiguration"/> to configure.</param>
    /// <param name="sentryOptions">The Sentry configuration options. If null or not enabled, Sentry will not be configured.</param>
    /// <returns>The modified <see cref="LoggerConfiguration"/> for chaining further configurations.</returns>
    private static LoggerConfiguration WriteToSentry(
        this LoggerConfiguration loggerConfig,
        SentryOptions? sentryOptions
    )
    {
        if (sentryOptions is null || !sentryOptions.Enabled)
        {
            return loggerConfig;
        }

        loggerConfig.WriteTo.Async(
            configure => configure.Sentry(
                options =>
                {
                    // Debug and higher are stored as breadcrumbs (default is Information)
                    options.MinimumBreadcrumbLevel = LogEventLevel.Information;
                    // Warning and higher is sent as event (default is Error)
                    options.MinimumEventLevel = LogEventLevel.Error;

                    options.Dsn = sentryOptions.Dsn;
                    options.TracesSampleRate = sentryOptions.TracesSampleRate;
                    options.SendDefaultPii = sentryOptions.SendDefaultPii;
                    options.AttachStacktrace = sentryOptions.AttachStackTrace;
                    options.Debug = sentryOptions.Debug;
                    options.DiagnosticLevel = Enum.Parse<SentryLevel>(sentryOptions.DiagnosticLevel);
                    // options.ProfilesSampleRate = sentryOption.ProfilesSampleRate; // <-- Configure Sentry to use cloud version: ProfilesSampleRate

                    options.TextFormatter = new MessageTemplateTextFormatter("[{MyTaskId}] {Message}");
                }
            )
        );

        return loggerConfig;
    }
}
