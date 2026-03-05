using Liberty.Reservation.Site.WebAPI.Middlewares;

namespace Liberty.Reservation.Site.WebAPI.Configurations;

public static class MiddlewareStartup
{
    public static IApplicationBuilder UseCustomMiddlewares(
        this IApplicationBuilder builder
    )
    {
        return builder
            .UseMiddleware<PreventDuplicateRequestMiddleware>();
    }

    public static IServiceCollection AddMiddlewareModule(
        this IServiceCollection services
    )
    {
        services.AddTransient<PreventDuplicateRequestMiddleware>();
        services.AddTransient<CheckFacilityAvailableMiddleware>();

        return services;
    }
}
