using Liberty.Cache;
using Liberty.GmoPaymentGateway;
using Liberty.Reservation.Site.Application.Auth;
using Liberty.Reservation.Site.WebAPI.Application.Boundaries.Grpc;
using Liberty.Reservation.Site.WebAPI.Configurations;
using Liberty.Reservation.Site.WebAPI.Middlewares;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.IdentityModel.Logging;

[assembly: ApiController]

namespace Liberty.Reservation.Site.WebAPI.Startup;

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
            .AddWebModule(environment)
            .AddSecurityModule(configuration)
            .AddProblemDetailsModule()
            .AddAppSettingsModule(configuration)
            .AddGmoPaymentGateway(configuration)
            .AddMiddlewareModule()
            .AddGrpcModule(configuration);

        services.AddEndpointsApiExplorer();
        services.AddRateLimiting();
        services.AddResponseCaching();

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
            context => context.Request.Headers.ContainsKey(SecurityContextAccessor.FacilityCodeHeaderKey)
                && context.Request.Headers.ContainsKey(SecurityContextAccessor.SiteCodeOfFacilityHeaderKey),
            appBuilder =>
            {
                appBuilder.UseMiddleware<CheckFacilityAvailableMiddleware>();
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

    public void ConfigureGrpcServices(
        WebApplication app,
        IHostEnvironment environment,
        IConfiguration configuration
    )
    {
        app.MapGrpcService<SiteProtoServiceEndpoint>();

        if (environment.IsDevelopment())
        {
            app.MapGrpcReflectionService();
        }
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
