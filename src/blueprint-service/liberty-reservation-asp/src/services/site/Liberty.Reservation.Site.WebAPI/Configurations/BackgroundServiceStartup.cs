using Liberty.Reservation.Site.WebAPI.Application.BackgroundServices;

namespace Liberty.Reservation.Site.WebAPI.Configurations;

public static class BackgroundServiceStartup
{
    public static IServiceCollection AddBackgroundServiceModule(
        this IServiceCollection services
    )
    {
        services.AddSingleton<BookingCreateSendEmailBackgroundService>();

        // Background Services
        services.AddHostedService<BookingCreateSendEmailBackgroundService>(
            provider =>
                provider.GetRequiredService<BookingCreateSendEmailBackgroundService>()
        );

        return services;
    }
}
