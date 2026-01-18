using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Domains.Services;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Employee.Application.Domains.Services;

public class SitePointRateService(
    ILogger<SitePointRateService> logger,
    ISitePointRateRepository sitePointRateRepository
) : BaseServiceRelation<SitePointRate>(logger, sitePointRateRepository), ISitePointRateService
{
    public async Task<IEnumerable<SitePointRate>> FindAllBySiteIdAsync(
        long siteId,
        CancellationToken cancellationToken = default
    )
    {
        var sitePointRates = await sitePointRateRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.SiteId == siteId)
            .ToListAsync(cancellationToken);

        return sitePointRates;
    }
}
