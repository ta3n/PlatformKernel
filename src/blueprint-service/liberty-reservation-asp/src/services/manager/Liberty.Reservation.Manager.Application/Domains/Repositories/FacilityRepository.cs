namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class FacilityRepository(
    ManagerDataContext dataContext
) : RepositoryBase<Facility>(dataContext), IFacilityRepository
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
}
