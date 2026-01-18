using Liberty.Reservation.Employee.WebAPI.Middlewares;

namespace Liberty.Reservation.Employee.WebAPI.Configurations;

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

        return services;
    }
}
