namespace Liberty.Reservation.User.Application.Domains.Services.Interfaces;

public interface ICacheManagementService
{
    void RemoveFacilityRelatedCache(
        long? facilityId
    );

    void RemoveAllFacilityBookingCache(
        long? bookingId
    );
}
