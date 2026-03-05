using Liberty.Reservation.Application.Domains.Services;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.Application.Domains.Services;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.ExternalServices.Membership.Facility.Services;
using Liberty.Reservation.Manager.WebAPI.Application.ExternalServices.Membership.Facility.Services.Implements;
using Liberty.Reservation.Manager.WebAPI.Application.Web.ApiService;

namespace Liberty.Reservation.Manager.WebAPI.Configurations;

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
        services.AddScoped<IExternalApiService, ExternalApiService>();
        services.AddSingleton<ICacheManagementService, CacheManagementService>();
        services.AddScoped<IMailTemplateService, MailTemplateService>();
        services.AddScoped<IFileService, FileService>();

        // Booking Services
        services.AddScoped<IBookingSecureUrlService, BookingSecureUrlService>();
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
        services.AddScoped<IBookingHoldManagementService, BookingHoldManagementService>();
        services.AddScoped<IBookingSystemConfigService, BookingSystemConfigService>();
        services.AddScoped<IBookingManagerModificationCheckerService, BookingManagerModificationCheckerService>();
        services.AddScoped<IBookingSearchService, BookingSearchService>();
        services.AddScoped<IBookingDetailService, BookingDetailService>();
        services.AddScoped<IBookingPaymentRestrictionService, BookingPaymentRestrictionService>();
        services.AddScoped<IBookingInventoryService, BookingInventoryService>();
        services.AddScoped<IBookingAuditCompareService, BookingAuditCompareService>();

        // Order Services
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IOrderBookingService, OrderBookingService>();

        // Facility Services
        services.AddScoped<IFacilityService, FacilityService>();
        services.AddScoped<IFacilityOptionItemService, FacilityOptionItemService>();
        services.AddScoped<IFacilityRoomGroupService, FacilityRoomGroupService>();
        services.AddScoped<IFacilityAppDateTypeService, FacilityAppDateTypeService>();
        services.AddScoped<IFacilityCalendarService, FacilityCalendarService>();
        services.AddScoped<IFacilityFileService, FacilityFileService>();
        services.AddScoped<IFacilityPersonAgeTypeService, FacilityPersonAgeTypeService>();
        services.AddScoped<IFacilityQuestionService, FacilityQuestionService>();
        services.AddScoped<IFacilityCancellationService, FacilityCancellationService>();
        services.AddScoped<IFacilityPlanService, FacilityPlanService>();
        services.AddScoped<IFacilityAllergenService, FacilityAllergenService>();
        services.AddScoped<IFacilityCategoryService, FacilityCategoryService>();
        services.AddScoped<ICheckFacilityService, CheckFacilityService>();

        // Room Group Services
        services.AddScoped<IRoomGroupService, RoomGroupService>();
        services.AddScoped<IRoomGroupBedTypeService, RoomGroupBedTypeService>();
        services.AddScoped<IRoomGroupCategoryService, RoomGroupCategoryService>();
        services.AddScoped<IRoomGroupSiteService, RoomGroupSiteService>();
        services.AddScoped<IRoomGroupAppDateService, RoomGroupAppDateService>();
        services.AddScoped<IFileRoomGroupService, FileRoomGroupService>();

        // Option Item Services
        services.AddScoped<IOptionItemService, OptionItemService>();
        services.AddScoped<IOptionItemCategoryService, OptionItemCategoryService>();
        services.AddScoped<IOptionItemQuestionService, OptionItemQuestionService>();
        services.AddScoped<IOptionItemAppDateService, OptionItemAppDateService>();
        services.AddScoped<IFileOptionItemService, FileOptionItemService>();

        // Calendar and Date Services
        services.AddScoped<IAppDateService, AppDateService>();
        services.AddScoped<IAppDateTypeService, AppDateTypeService>();
        services.AddScoped<ICalendarAppDateAppDateTypeService, CalendarAppDateAppDateTypeService>();
        services.AddScoped<ICalendarService, CalendarService>();

        // Category and Basic Services
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IQuestionService, QuestionService>();
        services.AddScoped<IBedTypeService, BedTypeService>();
        services.AddScoped<ISiteService, SiteService>();
        services.AddScoped<IFileCategoryService, FileCategoryService>();
        services.AddScoped<IAreaService, AreaService>();
        services.AddScoped<IMealTypeService, MealTypeService>();
        services.AddScoped<IAllergenService, AllergenService>();

        // Plan Services
        services.AddScoped<IPlanService, PlanService>();
        services.AddScoped<IPlanRoomGroupSiteAppDatePriceDataService, PlanRoomGroupSiteAppDatePriceDataService>();
        services.AddScoped<IPlanRoomGroupSiteAppDateService, PlanRoomGroupSiteAppDateService>();
        services.AddScoped<IPlanRoomGroupSiteDiscountDataService, PlanRoomGroupSiteDiscountDataService>();
        services.AddScoped<IPlanRoomGroupSitePersonAgeTypeService, PlanRoomGroupSitePersonAgeTypeService>();
        services.AddScoped<IPlanRoomGroupSiteService, PlanRoomGroupSiteService>();
        services.AddScoped<IPlanRoomGroupService, PlanRoomGroupService>();
        services.AddScoped<IPlanRoomGroupSiteAppDateTypePriceService, PlanRoomGroupSiteAppDateTypePriceService>();
        services.AddScoped<IPlanCategoryService, PlanCategoryService>();
        services.AddScoped<IPlanOptionItemService, PlanOptionItemService>();
        services.AddScoped<IPlanQuestionService, PlanQuestionService>();
        services.AddScoped<IPlanMealTypeService, PlanMealTypeService>();
        services.AddScoped<IFilePlanService, FilePlanService>();
        services.AddScoped<IPlanSiteService, PlanSiteService>();

        // Person Age Type Services
        services.AddScoped<IPersonAgeTypeService, PersonAgeTypeService>();
        services.AddScoped<IPersonAgeTypeSpaTaxDataService, PersonAgeTypeSpaTaxDataService>();

        // Cancellation Services
        services.AddScoped<ICancellationService, CancellationService>();
        services.AddScoped<ICancellationDataService, CancellationDataService>();
        services.AddScoped<IDataOfCancellationService, DataOfCancellationService>();
        services.AddScoped<ICancellationTableHtmlService, CancellationTableHtmlService>();
        services.AddScoped<IBookingCancellationService, BookingCancellationService>();
        services.AddScoped<IBookingOnlinePaymentService, BookingOnlinePaymentService>();
        services.AddScoped<IGmoChangeTranReportService, GmoChangeTranReportService>();

        return services;
    }
}
