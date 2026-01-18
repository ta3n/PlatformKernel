using Liberty.Reservation.Application.Domains.Services;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.Application.Domains.Services;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Services;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Services.Implements;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Web.ApiService;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Configurations;

public static class ServiceStartup
{
    public static IServiceCollection AddServiceModule(
        this IServiceCollection services
    )
    {
        services.AddScoped(typeof(IBaseService<>), typeof(BaseService<>));
        services.AddScoped(typeof(IBaseServiceRelation<>), typeof(BaseServiceRelation<>));

        services.AddScoped<IExternalApiService, ExternalApiService>();

        services.AddScoped<ICheckFacilityService, CheckFacilityService>();
        services.AddSingleton<ICacheManagementService, CacheManagementService>();
        services.AddScoped<IBookingDataAvailableService, BookingDataAvailableService>();
        services.AddScoped<IBookingSearchService, BookingSearchService>();
        services.AddScoped<IBookingCalendarPriceService, BookingCalendarPriceService>();
        services.AddKeyedScoped<IBookingPriceService, BookingPriceOfPlanService>("plan");
        services.AddKeyedScoped<IBookingPriceService, BookingPriceOfRoomService>("room");
        services.AddScoped<IBookingReservationService, BookingReservationService>();
        services.AddScoped<IBookingCheckAvailableService, BookingCheckAvailableService>();
        services.AddScoped<IOrderBookingService, OrderBookingService>();
        services.AddScoped<IBookingCreateService, BookingCreateService>();
        services.AddScoped<IBookingRoomAppDateService, BookingRoomAppDateService>();
        services.AddKeyedScoped<IBookingReservationPriceDataService, BookingReservationPriceDataOfPlanService>("plan");
        services.AddKeyedScoped<IBookingReservationPriceDataService, BookingReservationPriceDataOfRoomService>("room");
        services.AddScoped<IBookingDataService, BookingDataService>();
        services.AddScoped<IFacilityService, FacilityService>();
        services.AddScoped<IFacilitySiteService, FacilitySiteService>();
        services.AddScoped<IAppDateService, AppDateService>();
        services.AddScoped<IFacilityPersonAgeTypeService, FacilityPersonAgeTypeService>();
        services.AddScoped<IBookingDetailService, BookingDetailService>();
        services.AddScoped<IBookingOptionInventoryService, BookingOptionInventoryService>();
        services.AddScoped<IPlanService, PlanService>();
        services.AddScoped<IPlanRoomGroupService, PlanRoomGroupService>();
        services.AddScoped<IFilePlanService, FilePlanService>();
        services.AddScoped<IPlanRoomGroupSiteAppDateService, PlanRoomGroupSiteAppDateService>();
        services.AddScoped<IPlanRoomGroupSiteAppDatePriceDataService, PlanRoomGroupSiteAppDatePriceDataService>();
        services.AddScoped<IPlanRoomGroupSiteService, PlanRoomGroupSiteService>();
        services.AddScoped<IRoomGroupService, RoomGroupService>();
        services.AddScoped<IRoomGroupCategoryService, RoomGroupCategoryService>();
        services.AddScoped<IRoomGroupSiteService, RoomGroupSiteService>();
        services.AddScoped<IRoomGroupBedTypeService, RoomGroupBedTypeService>();
        services.AddScoped<IPlanDistributionService, PlanDistributionService>();
        services.AddScoped<IPlanPriceService, PlanPriceService>();
        services.AddScoped<ISiteService, SiteService>();
        services.AddScoped<IRoomAdjustmentStatusService, RoomAdjustmentStatusService>();
        services.AddScoped<IRoomGroupAppDateService, RoomGroupAppDateService>();
        services.AddScoped<IAppDateService, AppDateService>();
        services.AddScoped<IAppDateOfRoomService, AppDateOfRoomService>();
        services.AddScoped<IFacilityPlanService, FacilityPlanService>();
        services.AddScoped<IPriceDataService, PriceDataService>();

        return services;
    }
}
