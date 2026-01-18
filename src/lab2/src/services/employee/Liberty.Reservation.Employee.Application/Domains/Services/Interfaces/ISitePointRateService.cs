using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;

public interface ISitePointRateService : IBaseServiceRelation<SitePointRate>
{
    Task<IEnumerable<SitePointRate>> FindAllBySiteIdAsync(
        long siteId,
        CancellationToken cancellationToken = default
    );
}
