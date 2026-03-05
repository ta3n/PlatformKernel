using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Domains.Services;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Employee.Application.Domains.Services;

public class FacilitySiteService(
    ILogger<FacilitySiteService> logger,
    IFacilitySiteRepository facilitySiteRepository
) : BaseServiceRelation<FacilitySite>(logger, facilitySiteRepository), IFacilitySiteService
{
    public async Task<IEnumerable<FacilitySite>> FindAllByFacilityIdAsync(
        long facilityId,
        CancellationToken cancellationToken = default
    )
    {
        var facilitySites = await facilitySiteRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.FacilityId == facilityId)
            .ToListAsync(cancellationToken);

        return facilitySites;
    }
}
