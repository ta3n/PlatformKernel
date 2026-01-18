using Liberty.ApplicationShared.Extensions;
using Liberty.ApplicationShared.Settings;
using Liberty.Reservation.Employee.WebAPI.Configurations;
using Liberty.Reservation.Employee.WebAPI.Handlers;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.IdentityModel.Logging;
using Spectre.Console;

var webAppOptions = new WebApplicationOptions
{
    ContentRootPath = Directory.GetCurrentDirectory(),
    EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
    Args = args
};

var builder = WebApplication.CreateBuilder(webAppOptions);
var configuration = builder.Configuration;
var environment = builder.Environment;

var appInfo = configuration.GetOptionsExt<AppInfo>("App");

AnsiConsole.Write(
    new FigletText(appInfo.AppName ?? string.Empty)
        .Color(Color.MediumPurple)
);

builder.AddServiceDefaults();

builder.Services
    .AddDbContext(configuration)
    .AddInfrastructure(configuration)
    .AddServiceModule()
    .AddAutoMapperModule()
    .AddMediatRModule()
    .AddSwaggerModule()
    .AddWebModule()
    .AddSecurityModule(configuration)
    .AddProblemDetailsModule()
    .AddCustomOptions(configuration);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddRateLimiting();

builder.WebHost.UseDefaultServiceProvider(options => options.ValidateScopes = false);
builder.Services.Configure<HostOptions>(
    hostOptions =>
    {
        hostOptions.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
    }
);

// If using Kestrel:
builder.Services.Configure<KestrelServerOptions>(
    options =>
    {
        options.AllowSynchronousIO = true;
        IdentityModelEventSource.ShowPII = true;
    }
);

// If using IIS:
builder.Services.Configure<IISServerOptions>(
    options =>
    {
        options.AllowSynchronousIO = true;
        IdentityModelEventSource.ShowPII = true;
    }
);

// builder.Services.AddHostedService<InitializationService>();

var app = builder.Build();

// Error Handler
app.UseExceptionHandler(
    exceptionHandlerApp =>
    {
        exceptionHandlerApp.Run(
            async context => await ErrorHandler.HandleRequest(context)
        );
    }
);

app
    .UseApplicationSecurity(configuration)
    .UseApplicationSwagger()
    .UseApplicationWeb(environment)
    .UseApplicationProblemDetails(environment);

app.UseRateLimiter();

await app.RunAsync();
