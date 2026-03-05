using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

/// <summary>
/// プラン部屋リレーション
/// </summary>
public class PlanRoomGroup : EntityRelation
{
    /// <summary>
    /// プランID
    /// </summary>
    public long PlanId { get; set; }

    /// <summary>
    /// プラン
    /// </summary>
    public Plan? Plan { get; set; }

    /// <summary>
    /// 部屋ID
    /// </summary>
    public long RoomGroupId { get; set; }

    /// <summary>
    /// 部屋
    /// </summary>
    public RoomGroup? RoomGroup { get; set; }

    public bool IsUsed { get; set; }

    public ICollection<PlanRoomGroupSite>? PlanRoomGroupSites { get; set; }

    public ICollection<ReservationPlanRoomGroupAppDate>? ReservationPlanRoomGroupAppDates { get; set; }
}
