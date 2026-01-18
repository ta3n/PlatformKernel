using Asp.Versioning.ApiExplorer;
using Liberty.ApplicationShared.Extensions;
using Liberty.ApplicationShared.Settings;
using Liberty.Pagination.Swaggers;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Liberty.Reservation.Employee.WebAPI.Configurations;

public static class SwaggerStartup
{
    public static IServiceCollection AddSwaggerModule(
        this IServiceCollection services
    )
    {
        services.AddSwaggerGen(
            options =>
            {
                options.OperationFilter<PageableModelFilter>();
            }
        );
        services.ConfigureOptions<ConfigureSwaggerOptions>();
        services.ConfigureOptions<ConfigureSwaggerUiOptions>();

        return services;
    }

    public static IApplicationBuilder UseApplicationSwagger(
        this IApplicationBuilder app
    )
    {
        app.UseSwagger(c => { c.RouteTemplate = "{documentName}/api-docs"; });
        return app;
    }
}

public class ConfigureSwaggerOptions(
    IApiVersionDescriptionProvider provider,
    IConfiguration configuration
)
    : IConfigureOptions<SwaggerGenOptions>
{
    public void Configure(
        SwaggerGenOptions options
    )
    {
        var appInfo = configuration.GetOptionsExt<AppInfo>("App");

        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(
                description.GroupName,
                new OpenApiInfo
                {
                    Title = $"{appInfo.AppName}  - {description.ApiVersion}",
                    Version = description.ApiVersion.ToString(),
                    Description = $"app version: {appInfo.AppVersion}"
                }
            );
        }
    }
}

public class ConfigureSwaggerUiOptions(
    IApiVersionDescriptionProvider provider
) : IConfigureOptions<SwaggerUIOptions>
{
    public void Configure(
        SwaggerUIOptions options
    )
    {
        var descriptions = provider.ApiVersionDescriptions;
        foreach (var description in descriptions)
        {
            var url = $"/swagger/{description.GroupName}/swagger.json";
            var name = description.GroupName.ToUpperInvariant();
            options.SwaggerEndpoint(url, name);
        }
    }
}
