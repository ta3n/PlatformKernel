using BlueprintCqrs.Configuration;
using BlueprintCqrs.Infrastructure.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MediatR;
using BlueprintCqrs.Application;

[assembly: ApiController]

namespace BlueprintCqrs;

public class Startup : IStartup
{
    public virtual void Configure(
        IConfiguration configuration,
        IServiceCollection services
    )
    {
        services
            .AddAppSettingsModule(configuration);

        AddDatabase(configuration, services);
    }

    public virtual void ConfigureServices(
        IServiceCollection services,
        IHostEnvironment environment
    )
    {
        services.AddMediatR(typeof(ApplicationClassesAssemblyHelper));

        services
            .AddSecurityModule()
            .AddProblemDetailsModule()
            .AddAutoMapperModule()
            .AddSwaggerModule()
            .AddWebModule()
            .AddRepositoryModule()
            .AddServiceModule();
    }

    public virtual void ConfigureMiddleware(
        IApplicationBuilder app,
        IHostEnvironment environment
    )
    {
        var serviceProvider = app.ApplicationServices;
        var securitySettingsOptions = serviceProvider.GetRequiredService<IOptions<SecuritySettings>>();
        var securitySettings = securitySettingsOptions.Value;
        app
            .UseApplicationSecurity(securitySettings)
            .UseApplicationProblemDetails(environment)
            .UseApplicationDatabase(environment)
            .UseApplicationIdentity();
    }

    public virtual void ConfigureEndpoints(
        IApplicationBuilder app,
        IHostEnvironment environment
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
        services.AddDatabaseModule(configuration);
    }
}
