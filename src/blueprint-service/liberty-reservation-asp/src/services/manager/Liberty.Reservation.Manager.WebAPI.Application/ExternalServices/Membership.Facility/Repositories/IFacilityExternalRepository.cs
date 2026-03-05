using Liberty.ApplicationShared.Domains.Repositories;
using Liberty.Reservation.Manager.WebAPI.Application.ExternalServices.Membership.Facility.Dtos;

namespace Liberty.Reservation.Manager.WebAPI.Application.ExternalServices.Membership.Facility.Repositories;

public interface IFacilityExternalRepository : IGenericRepository<Models.Facility>
{
    Task<bool> CheckFacilityAvailableAsync(
        string facilityCode,
        CancellationToken cancellationToken = default
    );

    Task<FacilityInfoDto?> GetFacilityByIdAsync(
        long id,
        CancellationToken cancellationToken = default
    );
}
