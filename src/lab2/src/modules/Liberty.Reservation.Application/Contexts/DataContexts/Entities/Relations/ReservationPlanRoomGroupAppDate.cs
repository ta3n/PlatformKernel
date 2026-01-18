using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

/// <summary>
/// 予約部屋情報
/// </summary>
public class ReservationPlanRoomGroupAppDate : EntityRelation
{
    public long ReservationId { get; set; }

    public Data.Reservation? Reservation { get; set; }

    public long PlanId { get; set; }

    public Plan? Plan { get; set; }

    public long RoomGroupId { get; set; }
    public RoomGroup? RoomGroup { get; set; }

    public PlanRoomGroup? PlanRoomGroup => RoomGroup?.PlanRoomGroups?.FirstOrDefault(a => a.PlanId == PlanId);

    public long BookingDateId { get; set; }

    /// <summary>
    /// 日付順
    /// </summary>
    public int RestIndex { get; set; }

    /// <summary>
    /// 部屋順
    /// </summary>
    public int RoomGroupIndex { get; set; }

    /// <summary>
    /// 部屋代表者情報
    /// </summary>
    public CustomerInfo? CustomerInfo { get; set; }

    public TimeSpan? CheckInTime { get; set; }
    public TimeSpan? CheckOutTime { get; set; }

    /// <summary>
    /// 部屋販売数
    /// </summary>
    public RoomGroupAppDate? RoomGroupAppDate { get; set; }

    public ICollection<ReservationRoomGroupAppDatePersonAgeType>? ReservationRoomGroupAppDatePersonAgeTypes
    {
        get;
        set;
    }

    public ICollection<ReservationRoomGroupAppDateOptionItem>? ReservationRoomGroupAppDateOptionItems { get; set; }
}
