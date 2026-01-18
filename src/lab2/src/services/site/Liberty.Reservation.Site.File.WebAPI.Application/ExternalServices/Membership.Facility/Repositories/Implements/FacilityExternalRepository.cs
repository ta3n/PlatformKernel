using Liberty.ApplicationShared.Domains.Repositories;

namespace Liberty.Reservation.Site.File.WebAPI.Application.ExternalServices.Membership.Facility.Repositories.Implements;

public class FacilityExternalRepository(
    MembershipFacilityExternalDbContext dataContext
) : GenericRepository<Models.Facility>(dataContext), IFacilityExternalRepository
{
    public async Task<bool> CheckFacilityAvailableAsync(
        string facilityCode,
        CancellationToken cancellationToken = default
    )
    {
        return await GetQueryableWithAsNoTracking()
            .AnyAsync(
                x =>
                    x.Code == facilityCode,
                cancellationToken
            );
    }

    public async Task<Models.Facility?> GetFacilityAvailableByIdAsync(
        long facilityId,
        CancellationToken cancellationToken = default
    )
    {
        return await GetQueryableWithAsNoTracking()
            .Where(
                x =>
                    x.Id == facilityId
            )
            .SingleOrDefaultAsync(cancellationToken);
    }
}
