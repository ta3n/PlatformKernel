using Liberty.ApplicationShared.Domains.Repositories;
using Liberty.Pagination;
using Liberty.Reservation.Employee.WebAPI.Application.ExternalServices.Membership.Facility.Dtos;

namespace Liberty.Reservation.Employee.WebAPI.Application.ExternalServices.Membership.Facility.Repositories;

public interface IFacilityExternalRepository : IGenericRepository<Models.Facility>
{
    Task<FacilityInfoDto?> GetFacilityByIdAsync(
        long id,
        CancellationToken cancellationToken = default
    );

    Task<IPage<FacilityInfoDto>?> GetAllFacilitiesByPageAsync(
        IPageable pageable,
        CancellationToken cancellationToken = default
    );
}
