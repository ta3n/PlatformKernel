using Liberty.Reservation.Site.Public.WebAPI.Web.ApiService;

namespace Liberty.Reservation.Site.Public.WebAPI.Configurations;

public static class ServiceStartup
{
    public static IServiceCollection AddServiceModule(
        this IServiceCollection services
    )
    {
        services.AddScoped<IExternalPublicApiService, ExternalPublicApiService>();

        return services;
    }
}
