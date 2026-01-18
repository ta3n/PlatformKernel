using Liberty.ApplicationShared.Domains.Repositories;

namespace Liberty.Reservation.Site.File.WebAPI.Application.ExternalServices.Membership.Facility.Repositories;

public interface IFacilityExternalRepository : IGenericRepository<Models.Facility>
{
    Task<bool> CheckFacilityAvailableAsync(
        string facilityCode,
        CancellationToken cancellationToken = default
    );

    Task<Models.Facility?> GetFacilityAvailableByIdAsync(
        long facilityId,
        CancellationToken cancellationToken = default
    );
}
