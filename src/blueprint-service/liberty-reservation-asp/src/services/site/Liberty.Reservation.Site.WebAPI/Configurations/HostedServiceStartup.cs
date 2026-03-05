using Liberty.Reservation.Site.WebAPI.Application.HostedServices;
using Liberty.Reservation.Site.WebAPI.Initializations;

namespace Liberty.Reservation.Site.WebAPI.Configurations;

public static class HostedServiceStartup
{
    public static IServiceCollection AddHostedServiceModule(
        this IServiceCollection services
    )
    {
        services.AddHostedService<InitializationService>();
        services.AddHostedService<IntegrationEventOutboxHostedService>();
        return services;
    }
}
