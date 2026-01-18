namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class FacilityRepository(
    ManagerDataContext dataContext
) : RepositoryBase<Facility>(dataContext), IFacilityRepository
{
    public async Task<(bool isAvailable, long facilityId, TimeSpan facilityTimeZone)> CheckFacilityAvailableAsync(
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
                    facilityId = x.Id,
                    facilityTimeZone = x.TimeZone ?? DefaultValues.DefaultTimeZoneOffset
                }
            )
            .SingleOrDefaultAsync(cancellationToken);
        return result == null
            ? (isAvailable: false, facilityId: 0, facilityTimeZone: DefaultValues.DefaultTimeZoneOffset)
            : (result.isAvailable, result.facilityId, result.facilityTimeZone);
    }
}
