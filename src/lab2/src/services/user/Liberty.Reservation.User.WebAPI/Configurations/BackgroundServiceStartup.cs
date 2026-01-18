using Liberty.Reservation.User.WebAPI.Application.BackgroundServices;

namespace Liberty.Reservation.User.WebAPI.Configurations;

public static class BackgroundServiceStartup
{
    public static IServiceCollection AddBackgroundServiceModule(
        this IServiceCollection services
    )
    {
        services.AddSingleton<BookingConfirmSendEmailBackgroundService>();

        // Background Services
        services.AddHostedService<BookingConfirmSendEmailBackgroundService>(
            provider =>
                provider.GetRequiredService<BookingConfirmSendEmailBackgroundService>()
        );

        return services;
    }
}
