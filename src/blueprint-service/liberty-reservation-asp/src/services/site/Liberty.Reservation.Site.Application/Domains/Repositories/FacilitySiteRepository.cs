using Liberty.Reservation.Site.Application.Models;

namespace Liberty.Reservation.Site.Application.Domains.Repositories;

public class FacilitySiteRepository(
    SiteDataContext dbContext
) : RepositoryBase<FacilitySite>(dbContext), IFacilitySiteRepository
{
    public async Task<FacilitySiteModel?> GetSiteCodeAlreadyInFacilityAsync(
        string facilityCode,
        string siteCode,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = GetQueryableWithAsNoTracking()
            .Where(
                x => x.Facility!.Code == facilityCode
                    && x.Facility!.IsEnabled
                    && x.Site!.Code == siteCode
                    && x.Site!.IsEnabled
                    && x.IsEnabled
            )
            .Select(
                x => new FacilitySiteModel(
                    x.FacilityId,
                    x.SiteId
                )
            )
            .AsSingleQuery();

        return await queryable.FirstOrDefaultAsync(cancellationToken);
    }
}
