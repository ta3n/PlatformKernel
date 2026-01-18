using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

public class RoomGroupAppDateTypePriceData : EntityRelation
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

    public RoomGroupAppDateTypePriceData()
    {
    }

    public RoomGroupAppDateTypePriceData(
        RoomGroup roomGroup,
        AppDateType appDateType,
        PriceData priceData
    )
    {
        RoomGroupId = roomGroup.Id;
        RoomGroup = roomGroup;
        AppDateTypeId = appDateType.Id;
        AppDateType = appDateType;
        PriceDataId = priceData.Id;
        PriceData = priceData;
    }
}
