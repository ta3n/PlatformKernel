using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IFacilitySiteService : IBaseServiceRelation<FacilitySite>
{
    Task<IEnumerable<(long facilityId, long siteId)>> GetAllFacilitySitesByFacilityIdsAsync(
        long[] facilityIds,
        CancellationToken cancellationToken = default
    );
}
