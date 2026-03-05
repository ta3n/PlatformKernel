using Liberty.Entity;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

/// <summary>
/// 値引き情報
/// </summary>
public class DiscountData : EntityData
{
    /// <summary予約日〇日前～</summary>
    public int? StartPrevDay { get; set; }

    /// <summary予約日～〇日前</summary>
    public int? EndPrevDay { get; set; }

    /// <summary>
    /// 人数 最小
    /// </summary>
    public int? PersonMin { get; set; }

    /// <summary>
    /// 人数 最大
    /// </summary>
    public int? PersonMax { get; set; }

    /// <summary>
    /// 設定
    /// </summary>
    public float? Value { get; set; }

    public PriceSettingTypes PriceSettingType { get; set; }

    public bool InPrevDay(
        int prevDay
    )
    {
        var startPrevDay = StartPrevDay;
        var endPrevDay = EndPrevDay;

        if (startPrevDay > prevDay)
        {
            return false;
        }

        return !(endPrevDay < prevDay);
    }

    public bool InRange(
        int? persons
    )
    {
        var min = PersonMin;
        var max = PersonMax;

        if (min > persons)
        {
            return false;
        }

        return !(max < persons);
    }

    public int? GetDiscount(
        int? price
    )
    {
        if (Value is null)
        {
            return null;
        }

        if (price == null)
        {
            return null;
        }

        var value = Value;

        switch (PriceSettingType)
        {
            case PriceSettingTypes.None: return 0;
            case PriceSettingTypes.Percent:
                var val = value;
                var p = price;
                var val2 = p * val / 100;
                return (int)Math.Round(val2 ?? 0);
            case PriceSettingTypes.Price:
                return (int)value;
            case PriceSettingTypes.Discount:
            default:
                return null;
        }
    }

    /// <summary>
    /// プラン部屋サイトリレーション
    /// </summary>
    public ICollection<PlanRoomGroupSiteDiscountData>? PlanRoomGroupSiteDiscountData { get; set; }
}
