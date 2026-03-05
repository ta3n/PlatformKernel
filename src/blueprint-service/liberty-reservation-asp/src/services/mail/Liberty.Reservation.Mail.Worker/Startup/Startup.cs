using Liberty.Fax;
using Liberty.Reservation.Mail.Worker.Configurations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.IdentityModel.Logging;

[assembly: ApiController]

namespace Liberty.Reservation.Mail.Worker.Startup;

public sealed class Startup : IStartup
{
    public void Configure(
        IConfiguration configuration,
        IServiceCollection services
    )
    {
        // By default, ExcelDataReader throws a NotSupportedException "No data is available for encoding 1252." on .NET Core and .NET 5.0 or later. so we Register the code page provider during application initialization
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        services.AddAppSettingsModule(configuration);
    }

    public void ConfigureServices(
        IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment
    )
    {
        services
            .AddServiceModule()
            .AddTransportModule(configuration)
            .AddAppSettingsModule(configuration)
            .AddFaxImo(configuration);

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

    public void ConfigureMiddleware(
        IApplicationBuilder app,
        IHostEnvironment environment,
        IConfiguration configuration
    )
    {
    }

    public void ConfigureEndpoints(
        IApplicationBuilder app,
        IHostEnvironment environment,
        IConfiguration configuration
    )
    {
    }
}
