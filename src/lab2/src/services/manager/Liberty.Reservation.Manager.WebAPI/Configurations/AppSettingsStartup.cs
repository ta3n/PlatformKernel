using Liberty.ApplicationShared.Domains.Services.Mails;
using Liberty.ApplicationShared.Settings;
using Liberty.Reservation.Application.Settings;
using Liberty.Reservation.Manager.Application.Options;
using Liberty.Reservation.Manager.WebAPI.Application.Settings;

namespace Liberty.Reservation.Manager.WebAPI.Configurations;

public static class AppSettingsStartup
{
    public static IServiceCollection AddAppSettingsModule(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.Configure<AppInfo>(configuration.GetSection("App"));
        services.Configure<ServiceSetting>(configuration.GetSection("Services"));
        services.Configure<SmtpMailService.SmtpMailSetting>(configuration.GetSection("SmtpMail"));
        services.Configure<SecretKeySetting>(configuration.GetSection("SecretKey"));
        services.Configure<ClientSetting>(configuration.GetSection("Client"));
        services.Configure<MailTemplateSetting>(configuration.GetSection("MailTemplate"));
        services.Configure<IntegrationEventSetting>(configuration.GetSection("IntegrationEvent"));
        services.Configure<ManagerModifyOptions>(configuration.GetSection("ManagerModify"));

        return services;
    }
}
