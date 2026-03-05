using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Models;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class BookingDataAvailableService(
    ISecurityContextAccessor securityContextAccessor,
    IPlanRepository planRepository,
    IOptionItemRepository optionItemRepository,
    IRoomGroupRepository roomGroupRepository,
    IReservationRepository reservationRepository
) : IBookingDataAvailableService
{
    public async Task<long> CountOptionItemsByIdsAsync(
        long[] optionItemIds,
        DateTime dateCheck,
        CancellationToken cancellationToken = default
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        var queryable = optionItemRepository
            .GetQueryable()
            .Where(x => x.FacilityOptionItems!.Any(t => t.FacilityId == facilityId))
            .Where(x => optionItemIds.Contains(x.Id))
            .Where(x => x.IsEnabled)
            .Where(x => x.EnabledStart == null || x.EnabledStart <= dateCheck)
            .Where(x => x.EnabledEnd == null || x.EnabledEnd >= dateCheck)
            .Where(x => x.UseDisplayDate || x.DisplayDateStart == null || x.DisplayDateStart <= dateCheck)
            .Where(x => x.UseDisplayDate || x.DisplayDateEnd == null || x.DisplayDateEnd >= dateCheck)
            .Where(x => x.UseAcceptDate || x.AcceptDateStart == null || x.AcceptDateStart <= dateCheck)
            .Where(x => x.UseAcceptDate || x.AcceptDateEnd == null || x.AcceptDateEnd >= dateCheck);

        var existingCount = await queryable.CountAsync(
            cancellationToken
        );

        return existingCount;
    }

    public async Task<IEnumerable<OptionItemAppDate>> FindAllAppDatesOfOptionItemsAsync(
        long[] optionItemIds,
        long startAppDateCheckId,
        long endAppDateCheckId,
        CancellationToken cancellationToken = default
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        var queryable = optionItemRepository
            .GetQueryable()
            .Where(x => x.FacilityOptionItems!.Any(t => t.FacilityId == facilityId))
            .Where(x => optionItemIds.Contains(x.Id))
            .SelectMany(x => x.OptionItemAppDates!)
            .Where(x => x.AppDateId >= startAppDateCheckId)
            .Where(x => x.AppDateId <= endAppDateCheckId)
            .OrderBy(x => x.AppDateId);

        var data = await queryable.ToListAsync(
            cancellationToken
        );

        return data;
    }

    public async Task<IEnumerable<PlanRoomGroupSiteAppDate>> FindAllAppDatesOfSitesInPlanRoomGroupsAsync(
        long siteId,
        long planId,
        long roomGroupId,
        long startAppDateCheckId,
        long endAppDateCheckId,
        CancellationToken cancellationToken = default
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        var queryable = planRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.Id == planId)
            .Where(x => x.PlanSites!.Any(t => t.SiteId == siteId))
            .Where(x => x.FacilityPlans!.Any(t => t.FacilityId == facilityId))
            .Where(x => x.PlanRoomGroups!.Any(t => t.RoomGroupId == roomGroupId))
            .SelectMany(x => x.PlanRoomGroupSiteAppDates!)
            .Where(x => x.DateCalendar >= startAppDateCheckId)
            .Where(x => x.DateCalendar <= endAppDateCheckId);

        var data = await queryable.ToListAsync(
            cancellationToken
        );

        return data;
    }

    public async Task<IEnumerable<RoomGroupAppDate>> FindAllAppDatesOfRoomGroupAsync(
        long roomGroupId,
        long startAppDateCheckId,
        long endAppDateCheckId,
        CancellationToken cancellationToken = default
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        var queryalbe = roomGroupRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.Id == roomGroupId)
            .Where(x => x.FacilityRoomGroups!.Any(t => t.FacilityId == facilityId))
            .SelectMany(x => x.RoomGroupAppDates!)
            .Where(x => x.AppDateId >= startAppDateCheckId)
            .Where(x => x.AppDateId <= endAppDateCheckId);

        var data = await queryalbe.ToListAsync(
            cancellationToken
        );

        return data;
    }

    public async Task<IEnumerable<PlanRoomGroupSitePersonAgeType>> FindAllPersonAgeTypesOfSiteInPlanRoomAsync(
        long siteId,
        long planId,
        long roomGroupId,
        CancellationToken cancellationToken = default
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        var queryable = planRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.Id == planId)
            .Where(x => x.FacilityPlans!.Any(t => t.FacilityId == facilityId))
            .Where(x => x.PlanSites!.Any(t => t.SiteId == siteId))
            .Where(x => x.PlanRoomGroups!.Any(t => t.RoomGroupId == roomGroupId))
            .SelectMany(x => x.PlanRoomGroupSitePersonAgeTypes!)
            .Where(x => x.IsEnabled);

        var data = await queryable.ToListAsync(
            cancellationToken
        );

        return data;
    }

    public async Task<IEnumerable<PlanRoomGroupSiteAppDatePriceData>> FindAllAppDatePriceDataOfSiteInPlanRoomAsync(
        long siteId,
        long planId,
        long roomGroupId,
        long startAppDateCheckId,
        long endAppDateCheckId,
        CancellationToken cancellationToken = default
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        var queryable = planRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.Id == planId)
            .Where(x => x.FacilityPlans!.Any(t => t.FacilityId == facilityId))
            .Where(x => x.PlanSites!.Any(t => t.SiteId == siteId))
            .Where(x => x.PlanRoomGroups!.Any(t => t.RoomGroupId == roomGroupId))
            .SelectMany(x => x.PlanRoomGroupSiteAppDatePriceData!)
            .Include(x => x.PriceData!)
            .Where(x => x.DateCalendar >= startAppDateCheckId)
            .Where(x => x.DateCalendar <= endAppDateCheckId);

        var data = await queryable.ToListAsync(
            cancellationToken
        );

        return data;
    }

    public async Task<IEnumerable<PlanRoomGroupSiteDiscountData>> FindAllDiscountDataOfSiteInPlanRoomAsync(
        long siteId,
        long planId,
        long roomGroupId,
        CancellationToken cancellationToken = default
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        var queryalbe = planRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.Id == planId)
            .Where(x => x.FacilityPlans!.Any(t => t.FacilityId == facilityId))
            .Where(x => x.PlanSites!.Any(t => t.SiteId == siteId))
            .Where(x => x.PlanRoomGroups!.Any(t => t.RoomGroupId == roomGroupId))
            .SelectMany(x => x.PlanRoomGroupSiteDiscountData!)
            .Where(x => x.IsEnabled);

        var data = await queryalbe.ToListAsync(
            cancellationToken
        );

        return data;
    }

    public async Task<IEnumerable<PersonAgeType>> FindAllPersonAgeTypesAsync(
        long siteId,
        long planId,
        long roomGroupId,
        long[] personAgeTypeIds,
        CancellationToken cancellationToken = default
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        var queryable = planRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.Id == planId)
            .Where(x => x.FacilityPlans!.Any(t => t.FacilityId == facilityId))
            .Where(x => x.PlanSites!.Any(t => t.SiteId == siteId))
            .Where(x => x.PlanRoomGroups!.Any(t => t.RoomGroupId == roomGroupId))
            .SelectMany(x => x.PlanRoomGroupSitePersonAgeTypes!)
            .Where(x => x.IsRegardAdult)
            .Select(x => x.PersonAgeType!)
            .Where(x => personAgeTypeIds.Contains(x.Id))
            .Where(x => x.IsEnabled);

        var data = await queryable.ToListAsync(
            cancellationToken
        );

        return data;
    }

    public async Task<IEnumerable<ReservationPlanRoomGroupAppDate>> FindAllAppDatesOfReservationsAsync(
        long planId,
        long startAppDateCheckId,
        long endAppDateCheckId,
        CancellationToken cancellationToken = default
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        var queryable = reservationRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.PlanId == planId)
            .Where(x => x.Plan!.FacilityPlans!.Any(t => t.FacilityId == facilityId))
            .Where(x => x.IsReserved)
            .SelectMany(x => x.ReservationPlanRoomGroupAppDates!)
            .Where(x => x.AppDateId >= startAppDateCheckId)
            .Where(x => x.AppDateId <= endAppDateCheckId)
            .OrderBy(x => x.AppDateId);

        var data = await queryable.ToListAsync(
            cancellationToken
        );

        return data;
    }

    public async Task<BookingDataAvailableModel> GetDataAvailableAsync(
        BookingCreateRequest createRequest,
        CancellationToken cancellationToken = default
    )
    {
        var adjustRequest = createRequest.Adjust;
        var checkInDate = adjustRequest.CheckInDateId;
        var checkOutDate = adjustRequest.CheckOutDateId();
        var personAgeTypeIds = adjustRequest.GetPersonAgeTypeIds();
        var optionItemIds = adjustRequest.GetOptionItemIds();

        var availableAppDatesOfOptionItems = await FindAllAppDatesOfOptionItemsAsync(
            optionItemIds,
            checkInDate,
            checkOutDate,
            cancellationToken
        );

        var availableAppDatesOfSiteInPlanRoomGroup = await FindAllAppDatesOfSitesInPlanRoomGroupsAsync(
            createRequest.SiteId,
            createRequest.PlanId,
            createRequest.RoomGroupId,
            checkInDate,
            checkOutDate,
            cancellationToken
        );

        var availableAppDatesOfRoomGroup = await FindAllAppDatesOfRoomGroupAsync(
            createRequest.RoomGroupId,
            checkInDate,
            checkOutDate,
            cancellationToken
        );

        var availablePersonAgeTypesOfSiteInPlanRoom = await FindAllPersonAgeTypesOfSiteInPlanRoomAsync(
            createRequest.SiteId,
            createRequest.PlanId,
            createRequest.RoomGroupId,
            cancellationToken
        );

        var availableAppDatePriceDataOfSiteInPlanRoom = await FindAllAppDatePriceDataOfSiteInPlanRoomAsync(
            createRequest.SiteId,
            createRequest.PlanId,
            createRequest.RoomGroupId,
            checkInDate,
            checkOutDate,
            cancellationToken
        );

        var availableDiscountDataOfSiteInPlanRoom = await FindAllDiscountDataOfSiteInPlanRoomAsync(
            createRequest.SiteId,
            createRequest.PlanId,
            createRequest.RoomGroupId,
            cancellationToken
        );

        var availablePersonAgeTypes = (await FindAllPersonAgeTypesAsync(
            createRequest.SiteId,
            createRequest.PlanId,
            createRequest.RoomGroupId,
            personAgeTypeIds,
            cancellationToken
        )).ToList();
        if (availablePersonAgeTypes.Count == 0)
        {
            throw new ReservationHasNoAdultPersonAgeTypesException();
        }

        var availableAppDatesOfReservations = await FindAllAppDatesOfReservationsAsync(
            createRequest.PlanId,
            checkInDate,
            checkOutDate,
            cancellationToken
        );

        var bookingCheckAvailable = new BookingDataAvailableModel(
            availablePersonAgeTypes,
            availableAppDatesOfOptionItems,
            availableAppDatesOfSiteInPlanRoomGroup,
            availableAppDatesOfRoomGroup,
            availablePersonAgeTypesOfSiteInPlanRoom,
            availableAppDatePriceDataOfSiteInPlanRoom,
            availableDiscountDataOfSiteInPlanRoom,
            availableAppDatesOfReservations
        );

        return bookingCheckAvailable;
    }
}
