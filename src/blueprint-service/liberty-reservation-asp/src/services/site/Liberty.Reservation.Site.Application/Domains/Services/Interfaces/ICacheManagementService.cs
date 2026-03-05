namespace Liberty.Reservation.Site.Application.Domains.Services.Interfaces;

public interface ICacheManagementService
{
    void RemoveFacilityRelatedCache();

    void RemoveAllFacilityBookingCache(
        long? bookingId
    );
}
