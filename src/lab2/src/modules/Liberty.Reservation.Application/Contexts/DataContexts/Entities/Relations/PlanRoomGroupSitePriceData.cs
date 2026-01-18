using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

/// <summary>
/// プラン部屋サイト日別料金リレーション
/// </summary>
public class PlanRoomGroupSitePriceData : EntityRelation
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

    /// <summary>
    /// サイトID
    /// </summary>
    public long SiteId { get; set; }

    /// <summary>
    /// サイト
    /// </summary>
    public Site? Site { get; set; }

    /// <summary>
    /// 人数 最小
    /// </summary>
    public int? PersonMin { get; set; }

    /// <summary>
    /// 人数 最大
    /// </summary>
    public int? PersonMax { get; set; }

    /// <summary>
    /// 料金
    /// </summary>
    public int? Price { get; set; }

    public Facility? Facility => Plan?.FacilityPlans?.Select(a => a.Facility).FirstOrDefault();
}
