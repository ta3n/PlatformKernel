using Liberty.Reservation.Manager.File.WebAPI.Middlewares;

namespace Liberty.Reservation.Manager.File.WebAPI.Configurations;

public static class MiddlewareStartup
{
    public static IApplicationBuilder UseCheckFacilityMiddleware(
        this IApplicationBuilder builder
    )
    {
        return builder.UseMiddleware<CheckFacilityAvailableMiddleware>();
    }

    public static IServiceCollection AddMiddlewareModule(
        this IServiceCollection services
    )
    {
        services.AddTransient<CheckFacilityAvailableMiddleware>();

        return services;
    }
}
