using Liberty.ApplicationShared.Domains.Repositories;

namespace Liberty.Reservation.User.WebAPI.Application.ExternalServices.Membership.Facility.Repositories.Implements;

public class FacilityExternalRepository(
    MembershipFacilityExternalDbContext dataContext
)
    : GenericRepository<Models.Facility>(dataContext), IFacilityExternalRepository
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
        long id,
        CancellationToken cancellationToken = default
    )
    {
        return await GetQueryableWithAsNoTracking()
            .Where(
                x =>
                    x.Id == id && x.State == Models.FacilityStates.Public
            )
            .SingleOrDefaultAsync(cancellationToken);
    }
}
