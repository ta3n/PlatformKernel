using Liberty.ApplicationShared.Domains.Services.Mails;
using Liberty.ApplicationShared.Settings;
using Liberty.Reservation.Application.Settings;
using Liberty.Reservation.User.WebAPI.Application.Settings;

namespace Liberty.Reservation.User.WebAPI.Configurations;

public static class AppSettingsStartup
{
    public static IServiceCollection AddAppSettingsModule(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.Configure<AppInfo>(configuration.GetSection("App"));
        services.Configure<SmtpMailService.SmtpMailSetting>(configuration.GetSection("SmtpMail"));
        services.Configure<GMOPaymentSetting>(configuration.GetSection("GMOPayment"));
        services.Configure<SecretKeySetting>(configuration.GetSection("SecretKey"));
        services.Configure<MailTemplateSetting>(configuration.GetSection("MailTemplate"));
        services.Configure<IntegrationEventSetting>(configuration.GetSection("IntegrationEvent"));

        return services;
    }
}
