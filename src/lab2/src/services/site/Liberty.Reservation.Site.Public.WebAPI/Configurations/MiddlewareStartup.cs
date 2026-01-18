namespace Liberty.Reservation.Site.Public.WebAPI.Configurations;

public static class MiddlewareStartup
{
    public static IApplicationBuilder UseCustomMiddlewares(
        this IApplicationBuilder builder
    )
    {
        return builder;
    }

    public static IServiceCollection AddMiddlewareModule(
        this IServiceCollection services
    )
    {
        return services;
    }
}
