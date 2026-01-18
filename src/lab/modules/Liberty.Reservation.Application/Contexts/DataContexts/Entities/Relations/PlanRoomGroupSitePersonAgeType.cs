using Liberty.Entity;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

/// <summary>
/// プラン部屋サイト料金設定リレーション
/// プラン料金設定->「子供料金」に該当
/// 小学生＞高学年の受け入れを行うか、大人料金計算に含めるかなど
/// </summary>
public class PlanRoomGroupSitePersonAgeType : EntityRelation
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

    public long PersonAgeTypeId { get; set; }

    /// <summary>
    /// 施設年齢別設定
    /// </summary>
    public PersonAgeType? PersonAgeType { get; set; }

    /// <summary>
    /// 大人人数とみなすか？
    /// </summary>
    public bool IsRegardAdult { get; set; }

    /// <summary>
    /// 設定区分
    /// 何もしない(大人料金)、パーセント、金額、値引き、無料
    /// </summary>
    public PriceSettingTypes PriceSettingType { get; set; }

    /// <summary>
    /// 値 パーセントの場合: 1% は 0.01
    /// </summary>
    public float? Value { get; set; }

    public int? GetPrice(
        PriceData? priceData
    )
    {
        // null は設定がないなど受け入れられない状態
        if (priceData is null)
        {
            return null;
        }

        // 受け入れ
        if (!IsEnabled)
        {
            return null;
        }

        var price = priceData.Price;

        // 大人料金とみなす
        return IsRegardAdult ? price : GetPrice(price, PriceSettingType, Value);
    }

    public static int? GetPrice(
        int? price,
        PriceSettingTypes priceSettingType,
        float? value
    )
    {
        var val = value ?? 0f;
        var p = price ?? 0;

        switch (priceSettingType)
        {
            default:
            case PriceSettingTypes.None:
                return price;

            case PriceSettingTypes.Percent:
                var val2 = p * val / 100;

                return (int)Math.Round(val2);

            case PriceSettingTypes.Price:
                return (int)val;

            case PriceSettingTypes.Discount:
                return p - (int)val;
        }
    }

    public PlanRoomGroupSitePersonAgeType()
    {
    }

    public PlanRoomGroupSitePersonAgeType(
        Plan plan,
        RoomGroup roomGroup,
        Site site,
        PersonAgeType personAgetType
    )
    {
        PlanId = plan.Id;
        Plan = plan;
        RoomGroupId = roomGroup.Id;
        RoomGroup = roomGroup;
        SiteId = site.Id;
        Site = site;
        PersonAgeTypeId = personAgetType.Id;
        PersonAgeType = personAgetType;
    }

    public void SetIsMainSetting()
    {
        // isMainであれば、以下を強制する
        if (PersonAgeType is not { IsMain: true })
        {
            return;
        }

        IsEnabled = true; // 有効
        IsRegardAdult = true; // 大人として扱える
        PriceSettingType = PriceSettingTypes.None; // 料金カレンダーに準ずる
        Value = null; // 設定値は不要
    }
}
