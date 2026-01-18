using Liberty.Reservation.Site.Public.WebAPI.Settings;
using AppInfo = Liberty.Reservation.Site.Public.WebAPI.Settings.AppInfo;

namespace Liberty.Reservation.Site.Public.WebAPI.Configurations;

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
