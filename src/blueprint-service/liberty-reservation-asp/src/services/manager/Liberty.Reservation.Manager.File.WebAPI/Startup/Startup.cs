using Liberty.Cache;
using Liberty.MassTransit;
using Liberty.Media;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.File.WebAPI.Configurations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.IdentityModel.Logging;

[assembly: ApiController]

namespace Liberty.Reservation.Manager.File.WebAPI.Startup;

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
            .AddMassTransitCustom(configuration)
            .AddSwaggerModule()
            .AddWebModule()
            .AddSecurityModule(configuration)
            .AddProblemDetailsModule()
            .AddAppSettingsModule(configuration)
            .AddMiddlewareModule();

        services.AddTransportModule(configuration);

        services.AddEndpointsApiExplorer();
        services.AddRateLimiting();
        services.AddResponseCaching();

        services.AddAmazonS3(configuration);

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
            .UseApplicationProblemDetails(environment);

        app.UseWhen(
            context => context.Request.Headers.ContainsKey(SecurityContextAccessor.FacilityHeaderKey),
            appBuilder =>
            {
                appBuilder.UseCheckFacilityMiddleware();
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
        services.AddExternalDbContexts(configuration);
        services.AddDbContext(configuration);
    }
}
