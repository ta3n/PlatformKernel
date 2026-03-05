using Liberty.ApplicationShared.Domains.Services.Mails;
using Liberty.Reservation.Mail.Worker.Application.Services;
using Liberty.Reservation.Mail.Worker.Application.Web.ApiService;

namespace Liberty.Reservation.Mail.Worker.Configurations;

public static class ServiceStartup
{
    public static IServiceCollection AddServiceModule(
        this IServiceCollection services
    )
    {
        services.AddScoped<IExternalApiService, ExternalApiService>();

        services.AddScoped<IMailService, SmtpMailService>();

        services.AddScoped<IConvertHtmlToAsciiService, ConvertHtmlToAsciiService>();
        return services;
    }
}
