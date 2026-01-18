using Liberty.Reservation.Application.Contexts.DataContexts.Entities.AggregateLogs;
using Liberty.UnitOfWork;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using File = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.File;

namespace Liberty.Reservation.Application.Contexts.DataContexts;

public class ReservationDataContext(
    DbContextOptions<ReservationDataContext> options
) : AppDbContextBase<ReservationDataContext>(options)
{
    // Datas
    public DbSet<Allergen> Allergens { get; set; }
    public DbSet<AppDate> AppDates { get; set; }
    public DbSet<AppDateData> AppDateDatas { get; set; }
    public DbSet<AppDateType> AppDateTypes { get; set; }
    public DbSet<Area> Areas { get; set; }
    public DbSet<BedType> BedTypes { get; set; }
    public DbSet<Calendar> Calendars { get; set; }
    public DbSet<Cancellation> Cancellations { get; set; }
    public DbSet<CancellationData> CancellationDatas { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<ConsumptionTax> ConsumptionTaxes { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<DiscountData> DiscountDatas { get; set; }
    public DbSet<Facility> Facilities { get; set; }
    public DbSet<Favorite> Favorites { get; set; }
    public DbSet<FaxService> FaxServices { get; set; }
    public DbSet<File> Files { get; set; }
    public DbSet<GmoPaymentResultRequest> GmoPaymentResultRequests { get; set; }
    public DbSet<MealType> MealTypes { get; set; }
    public DbSet<OptionItem> OptionItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<PersonAgeType> PersonAgeTypes { get; set; }
    public DbSet<Plan> Plans { get; set; }
    public DbSet<Point> Points { get; set; }
    public DbSet<PointRate> PointRates { get; set; }
    public DbSet<Prefecture> Prefectures { get; set; }
    public DbSet<PriceData> PriceDatas { get; set; }
    public DbSet<Que> Ques { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Entities.Data.Reservation> Reservations { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<RoomGroup> RoomGroups { get; set; }
    public DbSet<Site> Sites { get; set; }
    public DbSet<SpaTaxGroup> SpaTaxGroups { get; set; }
    public DbSet<SpaTaxData> SpaTaxDatas { get; set; }
    public DbSet<SystemConfig> SystemConfigs { get; set; }
    public DbSet<CustomerInfo> UserInfos { get; set; }
    public DbSet<RoomAdjustmentStatus> RoomAdjustmentStatuses { get; set; }
    public DbSet<AdjustmentResult> AdjustmentResults { get; set; }

    // Relations
    public DbSet<AppDateAppDateData> AppDateAppDateDatas { get; set; }
    public DbSet<AppDateAppDateType> AppDateAppDateTypes { get; set; }
    public DbSet<ApplicationUserFavorite> ApplicationUserFavorites { get; set; }
    public DbSet<ApplicationUserPoint> ApplicationUserPoints { get; set; }
    public DbSet<CalendarAppDateAppDateType> CalendarAppDateAppDateTypes { get; set; }
    public DbSet<CancellationCancellationData> CancellationCancellationDatas { get; set; }
    public DbSet<FacilityAllergen> FacilityAllergens { get; set; }
    public DbSet<FacilityAppDateType> FacilityAppDateTypes { get; set; }
    public DbSet<FacilityCalendar> FacilityCalendars { get; set; }
    public DbSet<FacilityCancellation> FacilityCancellations { get; set; }
    public DbSet<FacilityCategory> FacilityCategories { get; set; }
    public DbSet<FacilityFaxService> FacilityFaxServices { get; set; }
    public DbSet<FacilityFile> FacilityFiles { get; set; }
    public DbSet<FacilityOptionItem> FacilityOptionItems { get; set; }
    public DbSet<FacilityPersonAgeType> FacilityPersonAgeTypes { get; set; }
    public DbSet<FacilityPlan> FacilityPlans { get; set; }
    public DbSet<FacilityQuestion> FacilityQuestions { get; set; }
    public DbSet<FacilityRoomGroup> FacilityRoomGroups { get; set; }
    public DbSet<FacilitySite> FacilitySites { get; set; }
    public DbSet<FacilitySpaTaxGroup> FacilitySpaTaxGroups { get; set; }
    public DbSet<FileCategory> FileCategories { get; set; }
    public DbSet<FileOptionItem> FileOptionItems { get; set; }
    public DbSet<FilePlan> FilePlans { get; set; }
    public DbSet<FileRoom> FileRooms { get; set; }
    public DbSet<FileRoomGroup> FileRoomGroups { get; set; }
    public DbSet<OptionItemAppDate> OptionItemAppDates { get; set; }
    public DbSet<OptionItemCategory> OptionItemCategories { get; set; }
    public DbSet<OptionItemQuestion> OptionItemQuestions { get; set; }
    public DbSet<OrderGmoPaymentResultRequest> OrderGmoPaymentResultRequests { get; set; }
    public DbSet<OrderReservation> OrderReservations { get; set; }
    public DbSet<PersonAgeTypeSpaTaxData> PersonAgeTypeSpaTaxDatas { get; set; }
    public DbSet<PlanCategory> PlanCategories { get; set; }
    public DbSet<PlanMealType> PlanMealTypes { get; set; }
    public DbSet<PlanOptionItem> PlanOptionItems { get; set; }
    public DbSet<PlanQuestion> PlanQuestions { get; set; }
    public DbSet<PlanRoomGroup> PlanRoomGroups { get; set; }
    public DbSet<PlanRoomGroupCancellation> PlanRoomGroupCancellations { get; set; }
    public DbSet<PlanRoomGroupSite> PlanRoomGroupSites { get; set; }
    public DbSet<PlanRoomGroupSiteAppDate> PlanRoomGroupSiteAppDates { get; set; }
    public DbSet<PlanRoomGroupSiteAppDatePriceData> PlanRoomGroupSiteAppDatePriceDatas { get; set; }
    public DbSet<PlanRoomGroupSiteAppDateTypePriceData> PlanRoomGroupSiteAppDateTypePriceDatas { get; set; }
    public DbSet<PlanRoomGroupSiteDiscountData> PlanRoomGroupSiteDiscountDatas { get; set; }
    public DbSet<PlanRoomGroupSitePersonAgeType> PlanRoomGroupSitePersonAgeTypes { get; set; }
    public DbSet<PlanSite> PlanSites { get; set; }
    public DbSet<ReservationPlanRoomGroupAppDate> ReservationPlanRoomGroupAppDates { get; set; }
    public DbSet<ReservationPoint> ReservationPoints { get; set; }
    public DbSet<ReservationQuestion> ReservationQuestions { get; set; }
    public DbSet<ReservationRoomGroupAppDateOptionItem> ReservationRoomGroupAppDateOptionItems { get; set; }
    public DbSet<ReservationRoomGroupAppDatePersonAgeType> ReservationRoomGroupAppDatePersonAgeTypes { get; set; }
    public DbSet<RoomGroupAppDate> RoomGroupAppDates { get; set; }
    public DbSet<RoomGroupAppDateTypePriceData> RoomGroupAppDateTypePriceDatas { get; set; }
    public DbSet<RoomGroupBedType> RoomGroupBedTypes { get; set; }
    public DbSet<RoomGroupCategory> RoomGroupCategories { get; set; }
    public DbSet<RoomGroupSite> RoomGroupSites { get; set; }
    public DbSet<RoomGroupSiteAppDate> RoomGroupSiteAppDates { get; set; }
    public DbSet<RoomGroupSiteAppDatePriceData> RoomGroupSiteAppDatePriceDatas { get; set; }
    public DbSet<RoomGroupSiteAppDateTypePriceData> RoomGroupSiteAppDateTypePriceDatas { get; set; }
    public DbSet<RoomRoomGroup> RoomRoomGroups { get; set; }
    public DbSet<SitePointRate> SitePointRates { get; set; }
    public DbSet<SpaTaxGroupSpaTaxData> SpaTaxGroupSpaTaxDatas { get; set; }
    public DbSet<BookingAggregateMailAuditLog> BookingAggregateMailAuditLogs { get; set; }
    public DbSet<BookingAggregateFaxAuditLog> BookingAggregateFaxAuditLogs { get; set; }
}
