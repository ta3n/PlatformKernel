using Liberty.ApplicationShared.Domains.Repositories;

namespace Liberty.Reservation.User.File.WebAPI.Application.ExternalServices.Membership.Facility.Repositories.Implements;

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
}
