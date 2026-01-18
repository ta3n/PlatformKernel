using Liberty.ApplicationShared.Domains.Repositories;

namespace Liberty.Reservation.User.WebAPI.Application.ExternalServices.Membership.Facility.Repositories;

public interface IFacilityExternalRepository : IGenericRepository<Models.Facility>
{
    Task<bool> CheckFacilityAvailableAsync(
        string facilityCode,
        CancellationToken cancellationToken = default
    );

    Task<Models.Facility?> GetFacilityAvailableByIdAsync(
        long id,
        CancellationToken cancellationToken = default
    );
}
