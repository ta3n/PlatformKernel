using Liberty.Reservation.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;

public interface IPlanRoomGroupService : IBaseServiceRelation<PlanRoomGroup>
{
    Task<PlanRoomGroup> FindByPlanIdAndRomTypeIdAsync(
        long planId,
        long romTypeId,
        CancellationToken cancellationToken = default
    );

    Task<PlanRoomGroup> FindByRoomGroupIdAsync(
        long roomGroupId,
        CancellationToken cancellationToken = default
    );

    Task<(IEnumerable<PlanRoomGroup> adds, IEnumerable<PlanRoomGroup> removes)> ChangePlanRoomGroupAsync(
        long planId,
        List<long> planRoomGroupIds,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );

    Task<Plan?> GetPlanWithRoomOnlyTypeAsync(
        long roomGroupId,
        long facilityId,
        CancellationToken cancellationToken = default
    );
}
