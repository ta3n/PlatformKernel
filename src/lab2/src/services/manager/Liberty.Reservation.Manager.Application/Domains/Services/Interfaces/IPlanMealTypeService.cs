using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IPlanMealTypeService : IBaseServiceRelation<PlanMealType>
{
    Task<(IEnumerable<PlanMealType> adds, IEnumerable<PlanMealType> removes, IEnumerable<PlanMealType> adjusts)>
        ChangePlanMealTypeAsync(
            long planId,
            List<PlanMealType> mealTypesOfPlan,
            bool autoSave = true,
            CancellationToken cancellationToken = default
        );
}
