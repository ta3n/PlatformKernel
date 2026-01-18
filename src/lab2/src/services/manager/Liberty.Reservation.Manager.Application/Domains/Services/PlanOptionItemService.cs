using Liberty.ApplicationShared.Utils;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class PlanOptionItemService(
    ILogger<PlanOptionItemService> logger,
    IPlanOptionItemRepository planOptionItemRepository
) : BaseServiceRelation<PlanOptionItem>(logger, planOptionItemRepository), IPlanOptionItemService
{
    private async Task<List<PlanOptionItem>> GetAllPlanOptionItemByPlanIdAsync(
        long planId,
        CancellationToken cancellationToken = default
    )
    {
        return await planOptionItemRepository.GetQueryableWithAsNoTracking()
            .Where(x => x.PlanId == planId)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IEnumerable<PlanOptionItem> adds, IEnumerable<PlanOptionItem> removes)>
        ChangePlanOptionItemAsync(
            long planId,
            List<long> planOptionItemIds,
            bool autoSave = true,
            CancellationToken cancellationToken = default
        )
    {
        var existingPlanOptionItem = (await GetAllPlanOptionItemByPlanIdAsync(
            planId,
            cancellationToken
        )).ToList();
        var updatePlanOptionItem = planOptionItemIds
            .Select(
                optionItemId => new PlanOptionItem
                {
                    PlanId = planId,
                    OptionItemId = optionItemId
                }
            )
            .ToList();

        var comparer = new DelegateEqualityComparer<PlanOptionItem>(
            (
                x,
                y
            ) => x?.OptionItemId == y?.OptionItemId && x!.PlanId == y?.PlanId,
            obj => obj.OptionItemId.GetHashCode() ^ obj.PlanId.GetHashCode()
        );

        var removePlanOptionItem = existingPlanOptionItem.Except(
            updatePlanOptionItem,
            comparer
        );
        var addPlanOptionItem = updatePlanOptionItem.Except(
            existingPlanOptionItem,
            comparer
        );

        var removes = await DeleteRangeAsync(
            removePlanOptionItem,
            autoSave,
            cancellationToken
        );

        var adds = await CreateRangeAsync(
            addPlanOptionItem,
            autoSave,
            cancellationToken
        );
        return (adds, removes);
    }
}
