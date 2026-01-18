using Liberty.Reservation.User.WebAPI.Application.HostedServices;
using Liberty.Reservation.User.WebAPI.Initializations;

namespace Liberty.Reservation.User.WebAPI.Configurations;

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
