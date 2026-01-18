namespace Liberty.Reservation.Site.Application.Domains.Services;

public class PlanRoomGroupService(
    ILogger<PlanRoomGroupService> logger,
    IPlanRoomGroupRepository planRoomGroupRepository
) : BaseServiceRelation<PlanRoomGroup>(logger, planRoomGroupRepository), IPlanRoomGroupService
{
    public async Task<Plan?> GetPlanWithRoomOnlyTypeAsync(
        long roomGroupId,
        long facilityId,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = planRoomGroupRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.RoomGroupId == roomGroupId)
            .Where(
                x => x.Plan!.FacilityPlans!.Any(
                    t => t.FacilityId == facilityId
                )
            )
            .Select(x => x.Plan);

        var plan = await queryable.FirstOrDefaultAsync(
            cancellationToken
        );

        return plan;
    }

    public async Task<bool?> IsAvailablePlanWithRoomAsync(
        long[] planIds,
        long[] roomGroupIds,
        long facilityId,
        PlanTypes planTypes,
        CancellationToken cancellationToken = default
    )
    {
        var queryable = planRoomGroupRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => planIds.Contains(x.PlanId))
            .Where(x => roomGroupIds.Contains(x.RoomGroupId))
            .Where(
                x => x.Plan!.FacilityPlans!.Any(
                    t => t.FacilityId == facilityId
                )
            )
            .Where(x => x.Plan!.PlanType == planTypes);

        var isAvailable = await queryable.AnyAsync(cancellationToken);

        return isAvailable;
    }
}
