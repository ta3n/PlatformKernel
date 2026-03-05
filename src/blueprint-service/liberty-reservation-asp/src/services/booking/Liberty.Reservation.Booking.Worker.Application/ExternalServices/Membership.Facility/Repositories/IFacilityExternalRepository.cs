using Liberty.ApplicationShared.Domains.Repositories;
using Liberty.Reservation.Booking.Worker.Application.ExternalServices.Membership.Facility.Dtos;

namespace Liberty.Reservation.Booking.Worker.Application.ExternalServices.Membership.Facility.Repositories;

public interface IFacilityExternalRepository : IGenericRepository<Models.Facility>
{
    Task<bool> CheckFacilityAvailableAsync(
        long facilityId,
        CancellationToken cancellationToken = default
    );

    Task<FacilityInfoDto?> GetFacilityByIdAsync(
        long id,
        CancellationToken cancellationToken = default
    );
}
