using Liberty.Cache;
using Liberty.GmoPaymentGateway;
using Liberty.Reservation.User.WebAPI.Configurations;
using Liberty.Reservation.User.WebAPI.Middlewares;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.IdentityModel.Logging;

[assembly: ApiController]

namespace Liberty.Reservation.User.WebAPI.Startup;

public class Startup : IStartup
{
    public virtual void Configure(
        IConfiguration configuration,
        IServiceCollection services
    )
    {
        // By default, ExcelDataReader throws a NotSupportedException "No data is available for encoding 1252." on .NET Core and .NET 5.0 or later. so we Register the code page provider during application initialization
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        services.AddAppSettingsModule(configuration);
        AddDatabase(configuration, services);

        services.AddHostedServiceModule();
        services.AddBackgroundServiceModule();
    }

    public virtual void ConfigureServices(
        IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment
    )
    {
        services
            .AddInfrastructure(configuration)
            .AddExternalRepositories()
            .AddServiceModule()
            .AddDistributedCache(configuration)
            .AddAutoMapperModule()
            .AddMediatRModule()
            .AddSwaggerModule()
            .AddWebModule()
            .AddSecurityModule(configuration)
            .AddProblemDetailsModule()
            .AddAppSettingsModule(configuration)
            .AddMiddlewareModule();

        services.AddEndpointsApiExplorer();
        services.AddRateLimiting();
        services.AddResponseCaching();
        services.AddGmoPaymentGateway(configuration);

        // If using Kestrel:
        services.Configure<KestrelServerOptions>(
            options =>
            {
                options.AllowSynchronousIO = true;
                IdentityModelEventSource.ShowPII = true;
            }
        );

        // If using IIS:
        services.Configure<IISServerOptions>(
            options =>
            {
                options.AllowSynchronousIO = true;
                IdentityModelEventSource.ShowPII = true;
            }
        );
    }

    public virtual void ConfigureMiddleware(
        IApplicationBuilder app,
        IHostEnvironment environment,
        IConfiguration configuration
    )
    {
        app
            .UseApplicationDatabase(environment)
            .UseApplicationSecurity(configuration)
            .UseApplicationProblemDetails(environment)
            .UseCustomMiddlewares();

        app.UseWhen(
            context => context.Request.Path.StartsWithSegments("/api/guests"),
            appBuilder =>
            {
                appBuilder.UseMiddleware<CheckGuestCodeValidMiddleware>();
            }
        );
    }

    public virtual void ConfigureEndpoints(
        IApplicationBuilder app,
        IHostEnvironment environment,
        IConfiguration configuration
    )
    {
        app
            .UseResponseCaching()
            .UseApplicationSwagger()
            .UseApplicationWeb(environment);
    }

    protected virtual void AddDatabase(
        IConfiguration configuration,
        IServiceCollection services
    )
    {
        services.AddDbContext(configuration);
        services.AddExternalDbContexts(configuration);
    }
}
