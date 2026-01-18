namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface ICacheManagementService
{
    void RemoveFacilityRelatedCache();

    void RemoveUserKeyRelatedCache();

    void RemoveAllFacilityBookingCache(
        long? bookingId
    );
}
