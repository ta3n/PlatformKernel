using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

/// <summary>
/// プラン部屋サイト日別料金リレーション
/// </summary>
public class PlanRoomGroupSiteAppDatePriceData : EntityRelation
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
    /// DateCalendar
    /// </summary>
    public long DateCalendar { get; set; }

    /// <summary>
    /// 料金ID
    /// </summary>
    public long PriceDataId { get; set; }

    /// <summary>
    /// 料金情報
    /// 〇〇人～〇〇人は、どの部屋タイプでいくらなのか
    /// </summary>
    public PriceData? PriceData { get; set; }

    public Facility? Facility => Plan?.FacilityPlans?.Select(a => a.Facility).FirstOrDefault();
}
