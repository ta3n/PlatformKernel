namespace Liberty.Reservation.Site.Application.Domains.Repositories;

public class FacilityRepository(
    SiteDataContext dbContext
) : RepositoryBase<Facility>(dbContext), IFacilityRepository
{
    public async Task<(bool isAvailable, long facilityId)> CheckFacilityAvailableAsync(
        string facilityCode,
        CancellationToken cancellationToken = default
    )
    {
        var result = await GetQueryableWithAsNoTracking()
            .Where(x => x.Code == facilityCode && x.IsEnabled)
            .Select(
                x => new
                {
                    isAvailable = x.IsEnabled,
                    facilityId = x.Id
                }
            )
            .SingleOrDefaultAsync(cancellationToken);
        return result == null ? (isAvailable: false, facilityId: 0) : (result.isAvailable, result.facilityId);
    }

    public async Task<long?> GetFacilityUpdatedTimeAvailableAsync(
        long? facilityId,
        DbContext? appDbContext,
        CancellationToken cancellationToken = default
    )
    {
        var queryable =
            (
                appDbContext is null
                    ? GetQueryableWithAsNoTracking()
                    : GetQueryableWithAsNoTracking(appDbContext)
            )
            .Where(x => x.Id == facilityId)
            .Select(x => x.UpdatedAt);

        var data = await queryable.FirstOrDefaultAsync(cancellationToken);

        return data;
    }
}
