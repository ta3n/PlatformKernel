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

    public long AppDateId { get; set; }
    public AppDate? AppDate { get; set; }

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
    public UserInfo? UserInfo { get; set; }

    public TimeSpan? CheckInTime { get; set; }
    public TimeSpan? CheckOutTime { get; set; }

    /// <summary>
    /// 部屋販売数
    /// </summary>
    public RoomGroupAppDate? RoomGroupAppDate { get; set; }

    public ICollection<ReservationRoomGroupAppDatePersonAgeType>? ReservationRoomGroupAppDatePersonAgeTypes;

    public ICollection<ReservationRoomGroupAppDateOptionItem>? ReservationRoomGroupAppDateOptionItems { get; set; }

    /// <summary>
    /// 宿泊料
    /// </summary>
    public int Amount
    {
        get
        {
            var amount = 0;
            amount += ReservationRoomGroupAppDatePersonAgeTypes?.Sum(
                a => a.TotalRoomGroupPrice
            ) ?? 0;
            amount += ReservationRoomGroupAppDateOptionItems?.Sum(
                a => a.TotalPrice
            ) ?? 0;

            return amount;
        }
    }

    /// <summary>
    /// 入湯税
    /// </summary>
    public int SpaTax
    {
        get
        {
            var amount = 0;
            amount += ReservationRoomGroupAppDatePersonAgeTypes?.Sum(
                a => a.TotalSpaTax
            ) ?? 0;
            return amount;
        }
    }

    public ReservationPlanRoomGroupAppDate()
    {
    }

    public ReservationPlanRoomGroupAppDate(
        Data.Reservation reservation,
        Plan plan,
        RoomGroup roomGroup,
        int roomGroupIndex,
        AppDate appDate,
        int restIndex
    )
    {
        ReservationId = reservation.Id;
        Reservation = reservation;

        PlanId = plan.Id;
        Plan = plan;
        RoomGroupId = roomGroup.Id;
        RoomGroup = roomGroup;
        AppDateId = appDate.Id;
        AppDate = appDate;

        RoomGroupIndex = roomGroupIndex;
        RestIndex = restIndex;
    }
}
