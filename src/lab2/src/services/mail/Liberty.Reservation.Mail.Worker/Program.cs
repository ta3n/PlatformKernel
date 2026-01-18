using Liberty.ApplicationShared.Constants;
using Liberty.ApplicationShared.Extensions;
using Liberty.ApplicationShared.Settings;
using Liberty.Reservation.Mail.Worker.Startup;
using Liberty.SentrySelfHosted;
using Liberty.Serilog;
using Liberty.ServiceDefaults;
using Liberty.SysException.Exceptions;
using Serilog;
using Spectre.Console;

try
{
    var webAppOptions = new WebApplicationOptions
    {
        ContentRootPath = Directory.GetCurrentDirectory(),
        EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
        Args = args
    };

    var builder = WebApplication.CreateBuilder(webAppOptions);
    var configuration = builder.Configuration;
    var appInfo = configuration.GetOptionsExt<AppInfo>("App");

    AnsiConsole.Write(
        new FigletText($"{appInfo.AppName} v{appInfo.AppVersion}")
            .Color(Color.MediumPurple)
    );

    Log.Information("Starting web host");

    builder.AddServiceDefaults();

    builder.AddSentryMonitoring(
        (
            sentryEvent,
            _
        ) => sentryEvent.Exception is AppException ? null : sentryEvent
    );

    builder.AddSerilogLogging(appInfo.AppName ?? string.Empty);

    builder.WebHost.UseDefaultServiceProvider(
        options => options.ValidateScopes = false
    );
    builder.Services.Configure<HostOptions>(
        hostOptions =>
        {
            hostOptions.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
        }
    );

    var startup = new Startup();

    startup.Configure(
        configuration,
        builder.Services
    );
    startup.ConfigureServices(
        builder.Services,
        builder.Configuration,
        builder.Environment
    );

    var app = builder.Build();

    app.UseSentryMonitoring(configuration);
    app.UseSerilogLogging();

    startup.ConfigureMiddleware(
        app,
        app.Environment,
        builder.Configuration
    );
    startup.ConfigureEndpoints(
        app.MapDefaultEndpoints(),
        app.Environment,
        builder.Configuration
    );

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    Environment.Exit(ServiceErrorCode.Failed);
}
finally
{
    await Log.CloseAndFlushAsync();
}
