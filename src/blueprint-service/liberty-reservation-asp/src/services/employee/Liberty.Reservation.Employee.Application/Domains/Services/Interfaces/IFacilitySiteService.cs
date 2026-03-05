using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;

public interface IFacilitySiteService : IBaseServiceRelation<FacilitySite>
{
    Task<IEnumerable<FacilitySite>> FindAllByFacilityIdAsync(
        long facilityId,
        CancellationToken cancellationToken = default
    );
}
