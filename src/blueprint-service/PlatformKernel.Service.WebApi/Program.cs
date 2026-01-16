using PlatformKernel.Service.WebApi.Startup;
using PlatformKernel.ApplicationShared.Constants;
using PlatformKernel.ApplicationShared.Extensions;
using PlatformKernel.ApplicationShared.Settings;
using PlatformKernel.Serilog;
using PlatformKernel.ServiceDefaults;
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

    builder.EnableResponseCompression();

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

    app.UseResponseCompressionDefaults();

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
