using Liberty.ApplicationShared.Domains.Services.Mails;
using Liberty.ApplicationShared.Settings;
using Liberty.MassTransit.Options;
using Liberty.Reservation.Application.Settings;
using Liberty.Reservation.Booking.Worker.Application.Options;
using Liberty.Reservation.Booking.Worker.Application.Settings;

namespace Liberty.Reservation.Booking.Worker.Configurations;

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
        services.Configure<MailTemplateSetting>(configuration.GetSection("MailTemplate"));
        services.Configure<SecretKeySetting>(configuration.GetSection("SecretKey"));
        services.Configure<RoomGroupAppDateOptions>(configuration.GetSection("PartitionQueue:SetRoomAppDate"));
        services.Configure<MessageQueueOptions>(configuration.GetSection("MessageQueueSettings"));
        services.Configure<AggregateAuditLogOption>(configuration.GetSection("PartitionQueue:AggregateAuditLog"));
        services.Configure<BookingSearchPrePrecomputeOption>(configuration.GetSection("PartitionQueue:BookingSearchPrePrecompute"));

        return services;
    }
}
