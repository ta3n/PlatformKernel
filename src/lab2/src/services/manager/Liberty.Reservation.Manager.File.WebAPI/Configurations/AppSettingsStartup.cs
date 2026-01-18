using Liberty.ApplicationShared.Settings;
using Liberty.Reservation.Manager.File.WebAPI.Application.Settings;

namespace Liberty.Reservation.Manager.File.WebAPI.Configurations;

public static class AppSettingsStartup
{
    public static IServiceCollection AddAppSettingsModule(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.Configure<AppInfo>(configuration.GetSection("App"));
        services.Configure<ServiceSetting>(configuration.GetSection("Services"));

        return services;
    }
}
