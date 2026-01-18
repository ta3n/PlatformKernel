using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Domains.Services;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Employee.Application.Domains.Services;

public class FacilityFaxSrvService(
    ILogger<FacilityFaxSrvService> logger,
    IFacilityFaxSrvRepository facilityFaxSrvRepository
) : BaseServiceRelation<FacilityFaxService>(logger, facilityFaxSrvRepository), IFacilityFaxSrvService
{
    public async Task<IEnumerable<FacilityFaxService>> FindAllByFacilityIdAsync(
        long facilityId,
        CancellationToken cancellationToken = default
    )
    {
        var facilityFaxServices = await facilityFaxSrvRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.FacilityId == facilityId)
            .ToListAsync(cancellationToken);

        return facilityFaxServices;
    }
}
