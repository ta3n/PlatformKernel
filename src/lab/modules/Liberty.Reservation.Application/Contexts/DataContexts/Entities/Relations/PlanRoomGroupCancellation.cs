using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

public class PlanRoomGroupCancellation : EntityRelation
{
    public long PlanId { get; set; }
    public Plan? Plan { get; set; }

    public long RoomGroupId { get; set; }
    public RoomGroup? RoomGroup { get; set; }

    public long CancellationId { get; set; }
    public Cancellation? Cancellation { get; set; }

    public PlanRoomGroupCancellation()
    {
    }

    public PlanRoomGroupCancellation(
        Plan plan,
        Cancellation cancellation
    )
    {
        PlanId = plan.Id;
        Plan = plan;
        CancellationId = cancellation.Id;
        Cancellation = cancellation;
    }
}
