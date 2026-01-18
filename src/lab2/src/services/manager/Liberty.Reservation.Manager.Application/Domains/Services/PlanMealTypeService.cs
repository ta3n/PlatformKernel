using Liberty.ApplicationShared.Utils;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class PlanMealTypeService(
    ILogger<PlanMealTypeService> logger,
    IPlanMealTypeRepository planMealTypeRepository
) : BaseServiceRelation<PlanMealType>(logger, planMealTypeRepository), IPlanMealTypeService
{
    private async Task<List<PlanMealType>> GetAllPlanMealTypeByPlanIdAsync(
        long planId,
        CancellationToken cancellationToken = default
    )
    {
        return await planMealTypeRepository.GetQueryableWithAsNoTracking()
            .Where(x => x.PlanId == planId)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IEnumerable<PlanMealType> adds, IEnumerable<PlanMealType> removes, IEnumerable<PlanMealType> adjusts)>
        ChangePlanMealTypeAsync(
            long planId,
            List<PlanMealType> mealTypesOfPlan,
            bool autoSave = true,
            CancellationToken cancellationToken = default
        )
    {
        var existingPlanMealType = (await GetAllPlanMealTypeByPlanIdAsync(
            planId,
            cancellationToken
        )).ToList();
        var updatePlanMealType = mealTypesOfPlan
            .Select(
                planMealType => new PlanMealType
                {
                    PlanId = planId,
                    MealTypeId = planMealType.MealTypeId,
                    MealTypeEatType = planMealType.MealTypeEatType
                }
            )
            .ToList();

        var comparer = new DelegateEqualityComparer<PlanMealType>(
            (
                x,
                y
            ) => x?.MealTypeId == y?.MealTypeId && x!.PlanId == y?.PlanId,
            obj => obj.PlanId.GetHashCode() ^ obj.MealTypeId.GetHashCode()
        );

        var removePlanMealType = existingPlanMealType.Except(
            updatePlanMealType,
            comparer
        );

        var addPlanMealType = updatePlanMealType.Except(
            existingPlanMealType,
            comparer
        );

        var adjustPlanMealType = updatePlanMealType.Except(
            addPlanMealType,
            comparer
        );

        var removes = await DeleteRangeAsync(
            removePlanMealType,
            autoSave,
            cancellationToken
        );

        var adds = await CreateRangeAsync(
            addPlanMealType,
            autoSave,
            cancellationToken
        );

        var adjusts = await UpdateRangeAsync(
            adjustPlanMealType,
            autoSave,
            cancellationToken
        );
        return (adds, removes, adjusts);
    }
}
