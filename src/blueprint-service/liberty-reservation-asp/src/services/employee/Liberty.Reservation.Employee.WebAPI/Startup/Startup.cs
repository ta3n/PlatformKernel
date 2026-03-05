using Liberty.Cache;
using Liberty.Fax;
using Liberty.Reservation.Employee.WebAPI.Configurations;
using Liberty.Reservation.Employee.WebAPI.Initializations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.IdentityModel.Logging;

[assembly: ApiController]

namespace Liberty.Reservation.Employee.WebAPI.Startup;

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

        services.AddHostedService<InitializationService>();
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

        services.AddFaxImo(configuration);

        // If using Kestrel:
        services.Configure<KestrelServerOptions>(
            options =>
            {
                options.AllowSynchronousIO = true;
                IdentityModelEventSource.ShowPII = true;
                options.Limits.MaxResponseBufferSize = 100 * 1024 * 1024; // 100MB
                options.Limits.MaxRequestBodySize = 10 * 1024 * 1024; // 10MB
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
            .UseResponseCaching()
            .UseApplicationDatabase(environment)
            .UseApplicationSecurity(configuration)
            .UseApplicationProblemDetails(environment)
            .UseCustomMiddlewares();
    }

    public virtual void ConfigureEndpoints(
        IApplicationBuilder app,
        IHostEnvironment environment,
        IConfiguration configuration
    )
    {
        app
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
