using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Manager.Application.Models;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IBookingDataAvailableService
{
    Task<long> CountOptionItemsByIdsAsync(
        long[] optionItemIds,
        DateTime dateCheck,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<OptionItemAppDate>> FindAllAppDatesOfOptionItemsAsync(
        long[] optionItemIds,
        long startAppDateCheckId,
        long endAppDateCheckId,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<PlanRoomGroupSiteAppDate>> FindAllAppDatesOfSitesInPlanRoomGroupsAsync(
        long siteId,
        long planId,
        long roomGroupId,
        long startAppDateCheckId,
        long endAppDateCheckId,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<RoomGroupAppDate>> FindAllAppDatesOfRoomGroupAsync(
        long roomGroupId,
        long startAppDateCheckId,
        long endAppDateCheckId,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<PlanRoomGroupSitePersonAgeType>> FindAllPersonAgeTypesOfSiteInPlanRoomAsync(
        long siteId,
        long planId,
        long roomGroupId,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<PlanRoomGroupSiteAppDatePriceData>> FindAllAppDatePriceDataOfSiteInPlanRoomAsync(
        long siteId,
        long planId,
        long roomGroupId,
        long startAppDateCheckId,
        long endAppDateCheckId,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<PlanRoomGroupSiteDiscountData>> FindAllDiscountDataOfSiteInPlanRoomAsync(
        long siteId,
        long planId,
        long roomGroupId,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<PersonAgeType>> FindAllPersonAgeTypesAsync(
        long siteId,
        long planId,
        long roomGroupId,
        long[] personAgeTypeIds,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<ReservationPlanRoomGroupAppDate>> FindAllAppDatesOfReservationsAsync(
        long planId,
        long startAppDateCheckId,
        long endAppDateCheckId,
        CancellationToken cancellationToken = default
    );

    Task<BookingDataAvailableModel> GetDataAvailableAsync(
        BookingCreateRequest createRequest,
        CancellationToken cancellationToken = default
    );
}
