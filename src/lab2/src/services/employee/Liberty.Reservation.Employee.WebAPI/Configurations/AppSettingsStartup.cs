using Liberty.ApplicationShared.Settings;
using static Liberty.ApplicationShared.Domains.Services.Mails.SmtpMailService;

namespace Liberty.Reservation.Employee.WebAPI.Configurations;

public static class AppSettingsStartup
{
    public static IServiceCollection AddAppSettingsModule(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.Configure<AppInfo>(configuration.GetSection("App"));
        services.Configure<SmtpMailSetting>(configuration.GetSection("SmtpMail"));

        return services;
    }
}
