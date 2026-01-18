using Liberty.Reservation.Manager.WebAPI.Middlewares;

namespace Liberty.Reservation.Manager.WebAPI.Configurations;

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
        services.AddTransient<CheckFacilityAvailableMiddleware>();

        return services;
    }
}
