using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IPlanSiteService : IBaseServiceRelation<PlanSite>
{
    Task<(IEnumerable<PlanSite> adds, IEnumerable<PlanSite> removes)> ChangeSitesOfPlanAsync(
        long planId,
        List<long> planSiteIds,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );
}
