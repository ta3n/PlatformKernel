using Liberty.ApplicationShared.Domains.Services.Mails;
using Liberty.Reservation.Mail.Worker.Application.Settings;

namespace Liberty.Reservation.Mail.Worker.Configurations;

public static class AppSettingsStartup
{
    public static IServiceCollection AddAppSettingsModule(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.Configure<ServiceSetting>(configuration.GetSection("Services"));
        services.Configure<SmtpMailService.SmtpMailSetting>(configuration.GetSection("SmtpMail"));

        return services;
    }
}
