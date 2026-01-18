using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

/// <summary>
/// 料金情報
/// </summary>
public class PriceData : EntityData
{
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

    public bool InRange(
        int?[] persons
    )
    {
        return persons.All(InRange);
    }

    public bool InRange(
        int? persons
    )
    {
        var min = PersonMin;
        var max = PersonMax;

        if (min is null)
        {
            return false;
        }

        if (max is null)
        {
            return false;
        }

        if (min > persons)
        {
            return false;
        }

        return !(max < persons);
    }

    /// <summary>
    /// パーセント指定か？
    /// </summary>
    //public bool IsPercent{ get; set; }

    public ICollection<RoomGroupAppDateTypePriceData>? RoomGroupAppDateTypePriceDatas { get; set; }

    /// <summary>
    /// プラン部屋サイト日別料金リレーション
    /// </summary>
    public ICollection<PlanRoomGroupSiteAppDatePriceData>? PlanRoomGroupSiteAppDatePriceData { get; set; }

    public ICollection<PlanRoomGroupSiteAppDateTypePriceData>? PlanRoomGroupSiteAppDateTypePriceData { get; set; }

    public ICollection<RoomGroupSiteAppDatePriceData>? RoomGroupSiteAppDatePriceData { get; set; }

    public ICollection<RoomGroupSiteAppDateTypePriceData>? RoomGroupSiteAppDateTypePriceData { get; set; }
}
