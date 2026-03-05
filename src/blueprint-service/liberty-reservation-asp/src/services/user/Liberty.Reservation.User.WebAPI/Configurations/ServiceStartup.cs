using Liberty.GmoPaymentGateway.Services;
using Liberty.GmoPaymentGateway.Services.Implementations;
using Liberty.Reservation.Application.Domains.Services;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.User.Application.Domains.Services;
using Liberty.Reservation.User.Application.Domains.Services.Interfaces;
using Liberty.Reservation.User.WebAPI.Application.Web.ApiService;

namespace Liberty.Reservation.User.WebAPI.Configurations;

public static class ServiceStartup
{
    public static IServiceCollection AddServiceModule(
        this IServiceCollection services
    )
    {
        // Base Services
        services.AddScoped(typeof(IBaseService<>), typeof(BaseService<>));
        services.AddScoped(typeof(IBaseServiceRelation<>), typeof(BaseServiceRelation<>));

        // Infrastructure Services
        services.AddScoped<IIntegrationEventOutboxService, IntegrationEventOutboxService>();
        services.AddSingleton<ICacheManagementService, CacheManagementService>();
        services.AddScoped<IExternalApiService, ExternalApiService>();
        services.AddScoped<IMailTemplateService, MailTemplateService>();

        // Booking Services
        services.AddScoped<IBookingHoldManagementService, BookingHoldManagementService>();
        services.AddScoped<IBookingCheckAvailableService, BookingCheckAvailableService>();
        services.AddScoped<IBookingCheckModifyInPriceService, BookingCheckModifyInPriceService>();
        services.AddScoped<IBookingDataAvailableService, BookingDataAvailableService>();
        services.AddScoped<IBookingRoomAppDateService, BookingRoomAppDateService>();
        services.AddKeyedScoped<IBookingReservationPriceDataService, BookingReservationPriceDataOfPlanService>("plan");
        services.AddKeyedScoped<IBookingReservationPriceDataService, BookingReservationPriceDataOfRoomService>("room");
        services.AddKeyedScoped<IBookingPriceService, BookingPriceOfPlanService>("plan");
        services.AddKeyedScoped<IBookingPriceService, BookingPriceOfRoomService>("room");
        services.AddScoped<IBookingReservationService, BookingReservationService>();
        services.AddScoped<IBookingCreateService, BookingCreateService>();
        services.AddScoped<IBookingDataService, BookingDataService>();
        services.AddScoped<IBookingSecureUrlService, BookingSecureUrlService>();
        services.AddScoped<IBookingSystemConfigService, BookingSystemConfigService>();
        services.AddScoped<IBookingSearchService, BookingSearchService>();
        services.AddScoped<IBookingDetailService, BookingDetailService>();
        services.AddScoped<IBookingPaymentRestrictionService, BookingPaymentRestrictionService>();
        services.AddScoped<IBookingInventoryService, BookingInventoryService>();
        services.AddScoped<IBookingOptionInventoryService, BookingOptionInventoryService>();

        // Order Services
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IOrderBookingService, OrderBookingService>();

        // Payment Services
        services.AddScoped<IOrderGmoPaymentService, OrderGmoPaymentService>();
        services.AddScoped<IGmoPaymentGatewayService, GmoPaymentGatewayService>();

        // Facility Services
        services.AddScoped<IFacilityService, FacilityService>();

        // General Services
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IBookingCancellationService, BookingCancellationService>();

        services.AddScoped<IBookingOnlinePaymentService, BookingOnlinePaymentService>();
        services.AddScoped<IGmoChangeTranReportService, GmoChangeTranReportService>();

        return services;
    }
}
