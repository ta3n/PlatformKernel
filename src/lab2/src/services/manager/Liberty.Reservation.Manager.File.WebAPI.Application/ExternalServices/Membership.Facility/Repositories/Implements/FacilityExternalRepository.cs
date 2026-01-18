using Liberty.ApplicationShared.Domains.Repositories;
using Liberty.Reservation.Manager.File.WebAPI.Application.ExternalServices.Membership.Facility.Models;

namespace Liberty.Reservation.Manager.File.WebAPI.Application.ExternalServices.Membership.Facility.Repositories.Implements;

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
                x => x.Code == facilityCode && x.State == FacilityStates.Public,
                cancellationToken
            );
    }
}
