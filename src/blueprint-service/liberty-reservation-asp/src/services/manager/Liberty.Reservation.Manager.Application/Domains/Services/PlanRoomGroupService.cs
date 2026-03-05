using Liberty.ApplicationShared.Utils;
using Liberty.Reservation.Application.Exceptions;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class PlanRoomGroupService(
    ILogger<PlanRoomGroupService> logger,
    IPlanRoomGroupRepository planRoomGroupRepository
) : BaseServiceRelation<PlanRoomGroup>(logger, planRoomGroupRepository), IPlanRoomGroupService
{
    public async Task<PlanRoomGroup> FindByPlanIdAndRomTypeIdAsync(
        long planId,
        long romTypeId,
        CancellationToken cancellationToken = default
    )
    {
        return await planRoomGroupRepository
                .GetQueryableWithAsNoTracking()
                .Where(
                    x => x.PlanId == planId && x.RoomGroupId == romTypeId
                )
                .FirstOrDefaultAsync(
                    cancellationToken
                )
            ?? throw new PlanNotfoundException();
    }

    public async Task<(IEnumerable<PlanRoomGroup> adds, IEnumerable<PlanRoomGroup> removes)>
        ChangePlanRoomGroupAsync(
            long planId,
            List<long> planRoomGroupIds,
            bool autoSave = true,
            CancellationToken cancellationToken = default
        )
    {
        var existingAllRoomsInPlan = (await GetAllPlanRoomGroupByPlanIdAsync(
            planId,
            cancellationToken
        )).ToList();

        var existingPlanRoomGroup = existingAllRoomsInPlan.Where(x => !x.IsDeleted).ToList();

        var updatePlanRoomGroup = planRoomGroupIds
            .Select(
                roomGroupId => new PlanRoomGroup
                {
                    PlanId = planId,
                    RoomGroupId = roomGroupId
                }
            )
            .ToList();

        var comparer = new DelegateEqualityComparer<PlanRoomGroup>(
            (
                x,
                y
            ) => x?.RoomGroupId == y?.RoomGroupId && x!.RoomGroupId == y?.RoomGroupId,
            obj => obj.PlanId.GetHashCode() ^ obj.RoomGroupId.GetHashCode()
        );

        var removeRoomsInPlan = existingPlanRoomGroup.Except(
                updatePlanRoomGroup,
                comparer
            )
            .ToList();

        var addRoomsInPlan = updatePlanRoomGroup.Except(
                existingAllRoomsInPlan,
                comparer
            )
            .ToList();

        var editRoomsInPlan = existingAllRoomsInPlan.Intersect(
                updatePlanRoomGroup,
                comparer
            )
            .ToList();

        foreach (var roomGroup in addRoomsInPlan)
        {
            roomGroup.IsEnabled = false;
        }

        removeRoomsInPlan.ForEach(x => x.IsDeleted = true);
        editRoomsInPlan.ForEach(x => x.IsDeleted = false);

        var removes = await UpdateRangeAsync(
            removeRoomsInPlan,
            autoSave,
            cancellationToken
        );

        var adds = await CreateRangeAsync(
            addRoomsInPlan,
            autoSave,
            cancellationToken
        );

        var edits = await UpdateRangeAsync(
            editRoomsInPlan,
            autoSave,
            cancellationToken
        );

        return ([.. adds, .. edits], removes);
    }

    private async Task<List<PlanRoomGroup>> GetAllPlanRoomGroupByPlanIdAsync(
        long planId,
        CancellationToken cancellationToken = default
    )
    {
        return await planRoomGroupRepository.GetQueryableWithAsNoTracking()
            .IgnoreQueryFilters()
            .Where(x => x.PlanId == planId)
            .ToListAsync(cancellationToken);
    }

    public async Task<PlanRoomGroup> FindByRoomGroupIdAsync(
        long roomGroupId,
        CancellationToken cancellationToken = default
    )
    {
        return await planRoomGroupRepository
                .GetQueryableWithAsNoTracking()
                .Where(
                    x => x.RoomGroupId == roomGroupId
                        && x.Plan!.PlanType == PlanTypes.RoomOnly
                )
                .FirstOrDefaultAsync(
                    cancellationToken
                )
            ?? throw new RoomGroupNotfoundException();
    }

    public async Task<Plan?> GetPlanWithRoomOnlyTypeAsync(
        long roomGroupId,
        long facilityId,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = planRoomGroupRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.RoomGroupId == roomGroupId)
            .Where(x => x.Plan!.FacilityPlans!.Any(t => t.FacilityId == facilityId))
            .Where(x => x.Plan!.PlanType == PlanTypes.RoomOnly)
            .Select(x => x.Plan);

        var plan = await queryable.SingleOrDefaultAsync(cancellationToken);

        return plan;
    }
}
