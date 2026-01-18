using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IPlanOptionItemService : IBaseServiceRelation<PlanOptionItem>
{
    Task<(IEnumerable<PlanOptionItem> adds, IEnumerable<PlanOptionItem> removes)> ChangePlanOptionItemAsync(
        long planId,
        List<long> planOptionItemIds,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );
}
