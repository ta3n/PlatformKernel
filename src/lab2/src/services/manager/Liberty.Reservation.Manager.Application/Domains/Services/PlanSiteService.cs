using Liberty.ApplicationShared.Utils;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class PlanSiteService(
    ILogger<PlanSiteService> logger,
    IPlanSiteRepository planSiteRepository
) : BaseServiceRelation<PlanSite>(logger, planSiteRepository), IPlanSiteService
{
    public async Task<(IEnumerable<PlanSite> adds, IEnumerable<PlanSite> removes)> ChangeSitesOfPlanAsync(
        long planId,
        List<long> planSiteIds,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        var existingSitesOfPlan = await planSiteRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.PlanId == planId)
            .ToListAsync(cancellationToken);

        var updateSitesOfPlan = planSiteIds
            .Select(
                siteId => new PlanSite
                {
                    PlanId = planId,
                    SiteId = siteId
                }
            )
            .ToList();

        var comparer = new DelegateEqualityComparer<PlanSite>(
            (
                x,
                y
            ) => x?.SiteId == y?.SiteId && x!.PlanId == y?.PlanId,
            obj => obj.PlanId.GetHashCode() ^ obj.SiteId.GetHashCode()
        );

        var removeSitesOfPlan = existingSitesOfPlan.Except(
            updateSitesOfPlan,
            comparer
        );

        var addSitesOfPlan = updateSitesOfPlan.Except(
            existingSitesOfPlan,
            comparer
        );

        var entitiesToRemove = await DeleteRangeAsync(
            removeSitesOfPlan,
            autoSave,
            cancellationToken
        );

        var entitiesToAdd = await CreateRangeAsync(
            addSitesOfPlan,
            autoSave,
            cancellationToken
        );

        return (entitiesToAdd, entitiesToRemove);
    }
}
