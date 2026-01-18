using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IPlanCategoryService : IBaseServiceRelation<PlanCategory>
{
    Task<(IEnumerable<PlanCategory> adds, IEnumerable<PlanCategory> removes)> ChangeCategoriesOfPlanAsync(
        long planId,
        List<long> planCategoryIds,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );
}
