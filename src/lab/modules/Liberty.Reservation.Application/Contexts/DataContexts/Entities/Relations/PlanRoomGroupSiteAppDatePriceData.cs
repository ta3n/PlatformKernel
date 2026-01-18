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
    /// 日付ID
    /// </summary>
    public long AppDateId { get; set; }

    /// <summary>
    /// 日付
    /// </summary>
    public AppDate? AppDate { get; set; }

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

    public PlanRoomGroupSiteAppDatePriceData()
    {
    }

    public PlanRoomGroupSiteAppDatePriceData(
        Plan plan,
        RoomGroup roomGroup,
        Site site,
        AppDate appDate,
        PriceData priceData
    )
    {
        PlanId = plan.Id;
        Plan = plan;
        RoomGroupId = roomGroup.Id;
        RoomGroup = roomGroup;
        SiteId = site.Id;
        Site = site;
        AppDateId = appDate.Id;
        AppDate = appDate;
        PriceDataId = priceData.Id;
        PriceData = priceData;
    }
}
