using PlatformKernel.Service.WebApi.Middlewares;

namespace PlatformKernel.Service.WebApi.Configurations;

public static class MiddlewareStartup
{
    public static IApplicationBuilder UseCustomMiddlewares(
        this IApplicationBuilder builder
    )
    {
        return builder.UseMiddleware<PreventDuplicateRequestMiddleware>();
    }

    public static IServiceCollection AddMiddlewareModule(
        this IServiceCollection services
    )
    {
        services.AddTransient<PreventDuplicateRequestMiddleware>();
        services.AddTransient<CheckTenantAvailableMiddleware>();

        return services;
    }
}
