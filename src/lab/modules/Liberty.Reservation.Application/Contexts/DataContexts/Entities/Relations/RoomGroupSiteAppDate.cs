using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

/// <summary>
/// 部屋サイト日別リレーション
/// </summary>
public class RoomGroupSiteAppDate : EntityRelation
{
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
    /// 自動割引設定を使用するか？
    /// </summary>
    public bool UseAutoDiscount { get; set; }

    /// <summary>
    /// ポイント付与率
    /// </summary>
    public float? PointRate { get; set; }

    public RoomGroupSiteAppDate()
    {
    }

    public RoomGroupSiteAppDate(
        RoomGroup roomGroup,
        Site site,
        AppDate appDate
    )
    {
        RoomGroupId = roomGroup.Id;
        RoomGroup = roomGroup;
        SiteId = site.Id;
        Site = site;
        AppDateId = appDate.Id;
        AppDate = appDate;
    }
}
