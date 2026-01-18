using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;

public interface IFacilityFaxSrvService : IBaseServiceRelation<FacilityFaxService>
{
    Task<IEnumerable<FacilityFaxService>> FindAllByFacilityIdAsync(
        long facilityId,
        CancellationToken cancellationToken = default
    );
}
