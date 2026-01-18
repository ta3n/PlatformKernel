using Liberty.ApplicationShared.Settings;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Settings;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Configurations;

public static class AppSettingsStartup
{
    public static IServiceCollection AddAppSettingsModule(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.Configure<AppInfo>(configuration.GetSection("App"));
        services.Configure<IdentitySetting>(configuration.GetSection("Identity"));
        services.Configure<ServiceSetting>(configuration.GetSection("Services"));
        services.Configure<KakusanApiSetting>(configuration.GetSection("KakusanApiSetting"));
        services.Configure<C001Setting>(configuration.GetSection("KakusanApiSetting:C001"));
        services.Configure<C002Setting>(configuration.GetSection("KakusanApiSetting:C002"));
        services.Configure<C003Setting>(configuration.GetSection("KakusanApiSetting:C003"));
        services.Configure<C004Setting>(configuration.GetSection("KakusanApiSetting:C004"));

        return services;
    }
}
