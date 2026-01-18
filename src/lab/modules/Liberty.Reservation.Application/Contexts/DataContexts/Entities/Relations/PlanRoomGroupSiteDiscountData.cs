using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

/// <summary>
/// プラン部屋サイト日別料金リレーション
/// </summary>
public class PlanRoomGroupSiteDiscountData : EntityRelation
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
    /// 値引きID
    /// </summary>
    public long DiscountDataId { get; set; }

    public DiscountData? DiscountData { get; set; }

    public PlanRoomGroupSiteDiscountData()
    {
    }

    public PlanRoomGroupSiteDiscountData(
        Plan plan,
        RoomGroup roomGroup,
        Site site,
        DiscountData discountData
    )
    {
        PlanId = plan.Id;
        Plan = plan;
        RoomGroupId = roomGroup.Id;
        RoomGroup = roomGroup;
        SiteId = site.Id;
        Site = site;
        DiscountDataId = discountData.Id;
        DiscountData = discountData;
    }
}
