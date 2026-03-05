using Liberty.Reservation.User.WebAPI.Middlewares;

namespace Liberty.Reservation.User.WebAPI.Configurations;

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
        services.AddTransient<CheckGuestCodeValidMiddleware>();

        return services;
    }
}
