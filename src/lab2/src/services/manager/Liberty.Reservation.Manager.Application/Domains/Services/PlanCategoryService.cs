using Liberty.ApplicationShared.Utils;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class PlanCategoryService(
    ILogger<PlanCategoryService> logger,
    IPlanCategoryRepository planCategoryRepository
) : BaseServiceRelation<PlanCategory>(logger, planCategoryRepository), IPlanCategoryService
{
    public async Task<(IEnumerable<PlanCategory> adds, IEnumerable<PlanCategory> removes)>
        ChangeCategoriesOfPlanAsync(
            long planId,
            List<long> planCategoryIds,
            bool autoSave = true,
            CancellationToken cancellationToken = default
        )
    {
        var existingCategoriesOfPlan = await planCategoryRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.PlanId == planId)
            .ToListAsync(cancellationToken);

        var updateCategoriesOfPlan = planCategoryIds
            .Select(
                categoryId => new PlanCategory
                {
                    PlanId = planId,
                    CategoryId = categoryId
                }
            )
            .ToList();

        var comparer = new DelegateEqualityComparer<PlanCategory>(
            (
                x,
                y
            ) => x?.CategoryId == y?.CategoryId && x!.PlanId == y?.PlanId,
            obj => obj.CategoryId.GetHashCode() ^ obj.PlanId.GetHashCode()
        );

        var removeCategoriesOfPlan = existingCategoriesOfPlan.Except(
            updateCategoriesOfPlan,
            comparer
        );
        var addCategoriesOfPlan = updateCategoriesOfPlan.Except(
            existingCategoriesOfPlan,
            comparer
        );

        var entitiesToRemove = await DeleteRangeAsync(
            removeCategoriesOfPlan,
            autoSave,
            cancellationToken
        );

        var entitiesToAdd = await CreateRangeAsync(
            addCategoriesOfPlan,
            autoSave,
            cancellationToken
        );
        return (entitiesToAdd, entitiesToRemove);
    }
}
