using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

namespace Liberty.Reservation.Application.Domains.Services.Interfaces;

public interface IBookingDataAvailableService
{
    Task<IEnumerable<OptionItemAppDate>> FindAllAppDatesOfOptionItemsAsync(
        long facilityId,
        long[] optionItemIds,
        long startAppDateCheckId,
        long endAppDateCheckId,
        long ignoreReservationId,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<RoomGroupAppDate>> FindAllAppDatesOfRoomGroupAsync(
        long facilityId,
        long roomGroupId,
        long startAppDateCheckId,
        long endAppDateCheckId,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<PlanRoomGroupSitePersonAgeType>> FindAllPersonAgeTypesOfSiteInPlanRoomAsync(
        long facilityId,
        long siteId,
        long planId,
        long roomGroupId,
        CancellationToken cancellationToken = default
    );

    Task<Contexts.DataContexts.Entities.Data.Reservation> GetReservationByUserCodeAsync(
        long id,
        string? userCode,
        CancellationToken cancellationToken = default
    );

    Task<Contexts.DataContexts.Entities.Data.Reservation> GetReservationByFacilityIdAsync(
        long id,
        long facilityId,
        CancellationToken cancellationToken = default
    );
}
