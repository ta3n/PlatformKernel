using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

public class RoomGroupSiteAppDateTypePriceData : EntityRelation
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
    /// 掲載先ID
    /// </summary>
    public long SiteId { get; set; }

    /// <summary>
    /// 掲載先
    /// </summary>
    public Site? Site { get; set; }

    /// <summary>
    /// 日付種別ID
    /// </summary>
    public long AppDateTypeId { get; set; }

    /// <summary>
    /// 日付種別
    /// </summary>
    public AppDateType? AppDateType { get; set; }

    /// <summary>
    /// 料金ID
    /// </summary>
    public long PriceDataId { get; set; }

    /// <summary>
    /// 料金情報
    /// 〇〇人～〇〇人は、どの部屋タイプでいくらなのか
    /// </summary>
    public PriceData? PriceData { get; set; }
}
