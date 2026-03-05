namespace Liberty.Reservation.Site.Application.Domains.Services.Interfaces;

public interface IPlanRoomGroupService : IBaseServiceRelation<PlanRoomGroup>
{
    Task<Plan?> GetPlanWithRoomOnlyTypeAsync(
        long roomGroupId,
        long facilityId,
        CancellationToken cancellationToken = default
    );

    Task<bool?> IsAvailablePlanWithRoomAsync(
        long[] planIds,
        long[] roomGroupIds,
        long facilityId,
        PlanTypes planTypes,
        CancellationToken cancellationToken = default
    );
}
