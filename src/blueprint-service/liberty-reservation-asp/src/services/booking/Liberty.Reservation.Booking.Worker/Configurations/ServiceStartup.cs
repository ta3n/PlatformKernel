using Liberty.ApplicationShared.Domains.Services.Mails;
using Liberty.Media.Services;
using Liberty.Reservation.Application.Domains.Services;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.Application.Domains.Services;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Booking.Worker.Application.Web.ApiService;
using Liberty.Cache.Services;

namespace Liberty.Reservation.Booking.Worker.Configurations;

public static class ServiceStartup
{
    public static IServiceCollection AddServiceModule(
        this IServiceCollection services
    )
    {
        services.AddScoped(typeof(IBaseService<>), typeof(BaseService<>));
        services.AddScoped(typeof(IBaseServiceRelation<>), typeof(BaseServiceRelation<>));

        services.AddScoped<IExternalApiService, ExternalApiService>();

        services.AddScoped<IMailService, SmtpMailService>();
        services.AddScoped<IMailTemplateService, MailTemplateService>();
        services.AddScoped<IFacilityService, FacilityService>();
        services.AddScoped<IBookingReservationService, BookingReservationService>();
        services.AddScoped<IAwsS3Service, AwsS3Service>();
        services.AddScoped<IBookingSecureUrlService, BookingSecureUrlService>();
        services.AddSingleton<ICacheManagementService, CacheManagementService>();
        services.AddSingleton<ICacheService, CacheService>();
        services.AddScoped<IBookingRoomAppDateService, BookingRoomAppDateService>();
        services.AddScoped<IAdjustmentResultService, AdjustmentResultService>();
        services.AddScoped<IRoomAdjustmentStatusService, RoomAdjustmentStatusService>();
        services.AddScoped<IAppDateService, AppDateService>();

        return services;
    }
}
