using AngleSharp.Io;
using Asp.Versioning.ApiExplorer;
using Liberty.ApplicationShared.Extensions;
using Liberty.ApplicationShared.Settings;
using Liberty.Entity.Utils;
using Liberty.Pagination.Swaggers;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
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
                //options.OperationFilter<AcceptLanguageSelectHeaderParameter>();

                var securitySchema = new OpenApiSecurityScheme
                {
                    Description = "Using the Authorization header with the Bearer scheme.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };

                options.AddSecurityDefinition(
                    "Bearer",
                    securitySchema
                );

                options.AddSecurityRequirement(
                    new OpenApiSecurityRequirement { { securitySchema, ["Bearer"] } }
                );
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
        foreach (var groupName in descriptions.Select(x => x.GroupName))
        {
            var url = $"/swagger/{groupName}/swagger.json";
            var name = groupName.ToUpperInvariant();
            options.SwaggerEndpoint(url, name);
        }
    }
}

public class AcceptLanguageSelectHeaderParameter : IOperationFilter
{
    public void Apply(
        OpenApiOperation operation,
        OperationFilterContext context
    )
    {
        operation.Parameters.Add(
            new OpenApiParameter
            {
                Name = HeaderNames.AcceptLanguage,
                In = ParameterLocation.Header,
                Description = "Accept-Language header",
                Schema = new OpenApiSchema
                {
                    Type = "string",
                    Default = new OpenApiString(LanguageHeaderUtil.DefaultAcceptLanguage)
                },
                Required = true
            }
        );
    }
}
