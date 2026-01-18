using Liberty.ApplicationShared.Settings;

namespace Liberty.Reservation.User.File.WebAPI.Configurations;

public static class AppSettingsStartup
{
    public static IServiceCollection AddAppSettingsModule(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.Configure<AppInfo>(configuration.GetSection("App"));

        return services;
    }
}
