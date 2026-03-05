using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IPlanRoomGroupSiteService : IBaseServiceRelation<PlanRoomGroupSite>
{
    Task<List<PlanRoomGroupSite>> FindAllByPlanIdAndRomTypeIdAsync(
        long planId,
        long romTypeId,
        long siteId,
        CancellationToken cancellationToken = default
    );

    Task<PlanRoomGroupSite?> FindByPlanIdAndRomTypeIdAsync(
        long planId,
        long romTypeId,
        long siteId,
        CancellationToken cancellationToken = default
    );

    Task<PlanRoomGroupSite> UpdateMinimumPriceAsync(
        long planId,
        long romGroupId,
        long siteId,
        PlanRoomGroupSite entityToUpdate,
        CancellationToken cancellationToken = default
    );
}
