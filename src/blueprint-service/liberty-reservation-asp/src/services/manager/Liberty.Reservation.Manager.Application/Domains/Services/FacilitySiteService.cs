using Liberty.Cache.Services;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class FacilitySiteService(
    ILogger<FacilitySiteService> logger,
    ICacheService cacheService,
    IFacilitySiteRepository repository
) : BaseServiceRelation<FacilitySite>(logger, repository), IFacilitySiteService
{
    public async Task<IEnumerable<(long facilityId, long siteId)>> GetAllFacilitySitesByFacilityIdsAsync(
        long[] facilityIds,
        CancellationToken cancellationToken = default
    )
    {
        var cacheKey = string.Format(CacheKeys.RssFacilitySitesPrefixKey, string.Join("-", facilityIds));
        var cachedFacilitySites = await cacheService.GetAsync<IEnumerable<(long facilityId, long siteId)>>(
            cacheKey,
            cancellationToken
        );
        if (cachedFacilitySites is not null)
        {
            return cachedFacilitySites;
        }

        var queryable = repository
            .GetQueryableWithAsNoTracking()
            .Where(x => facilityIds.Contains(x.FacilityId))
            .Where(x => x.IsEnabled)
            .Select(
                x => new
                {
                    x.FacilityId,
                    x.SiteId
                }
            )
            .OrderBy(x => x.FacilityId);

        var facilitySites = await queryable.ToListAsync(cancellationToken)
            ?? throw new FacilitySiteNotFoundException();

        var dataFacilitySites = facilitySites
            .Select(x => (x.FacilityId, x.SiteId))
            .ToList();

        await cacheService.SetAsync(
            cacheKey,
            dataFacilitySites,
            cancellationToken
        );

        return dataFacilitySites;
    }
}
