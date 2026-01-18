using Liberty.GmoPaymentGateway.Services;
using Liberty.GmoPaymentGateway.Services.Implementations;
using Liberty.Reservation.Application.Domains.Services;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Site.Application.Domains.Services;
using Liberty.Reservation.Site.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Site.WebAPI.Application.Web.ApiService;

namespace Liberty.Reservation.Site.WebAPI.Configurations;

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
        services.AddScoped<IBookingOptionInventoryService, BookingOptionInventoryService>();
        services.AddScoped<IBookingReservationService, BookingReservationService>();
        services.AddScoped<IBookingCreateService, BookingCreateService>();
        services.AddScoped<IBookingDataService, BookingDataService>();
        services.AddScoped<IBookingSecureUrlService, BookingSecureUrlService>();
        services.AddScoped<IBookingSearchService, BookingSearchService>();
        services.AddScoped<IBookingDetailService, BookingDetailService>();
        services.AddScoped<IBookingCalendarPriceService, BookingCalendarPriceService>();
        services.AddScoped<IBookingInventoryService, BookingInventoryService>();

        // Order Services
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IOrderBookingService, OrderBookingService>();

        // Payment Services
        services.AddScoped<IGmoPaymentGatewayService, GmoPaymentGatewayService>();

        // Facility and Plan Services
        services.AddScoped<IFacilityService, FacilityService>();
        services.AddScoped<IPlanService, PlanService>();
        services.AddScoped<IPlanRoomGroupService, PlanRoomGroupService>();

        // Reservation Services
        services.AddScoped<IReservationService, ReservationService>();
        services.AddScoped<IReservationPlanRoomGroupAppDateService, ReservationPlanRoomGroupAppDateService>();
        services.AddScoped<IReservationQuestionService, ReservationQuestionService>();
        services.AddScoped<IReservationRoomGroupAppDateOptionItemService, ReservationRoomGroupAppDateOptionItemService>();
        services.AddScoped<IReservationRoomGroupAppDatePersonAgeTypeService, ReservationRoomGroupAppDatePersonAgeTypeService>();

        // General Services
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IQuestionService, QuestionService>();
        services.AddScoped<ICheckChangedService, CheckChangedService>();
        services.AddScoped<IBookingCancellationService, BookingCancellationService>();
        services.AddScoped<IPermissionService, PermissionService>();

        return services;
    }
}
