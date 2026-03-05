using Liberty.Entity;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Newtonsoft.Json;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

/// <summary>
/// 部屋情報
/// </summary>
public class RoomGroup : EntityData
{
    /// <summary>
    /// 部屋名
    /// </summary>
    public MultilingualText? Name { get; set; }

    /// <summary>
    /// 説明
    /// </summary>
    public MultilingualText? Description { get; set; }

    public bool IsDescriptionVisible { get; set; }
    public MultilingualText? Overview { get; set; }
    public bool IsOverviewVisible { get; set; }

    /// <summary>
    /// 基本提供数
    /// </summary>
    public int BaseNumber { get; set; }

    /// <summary>
    /// OTAに使われる宿でユニークとなる部屋識別子
    /// </summary>
    public string? GroupName { get; set; }

    /// <summary>
    /// 部屋定員 最小
    /// </summary>
    public int? CapacityMin { get; set; }

    /// <summary>
    /// 部屋定員 最大
    /// </summary>
    public int? CapacityMax { get; set; }

    /// <summary>
    /// 部屋広さ
    /// </summary>
    public float? Size { get; set; }

    public bool IsRoomSizeVisible { get; set; }

    public bool IsBedTypeVisible { get; set; }

    /// <summary>
    ///  部屋広さ単位
    /// </summary>
    public RoomGroupSizeUnitTypes RoomGroupSizeUnitType { get; set; }

    public string? Tag { get; set; }

    /// <summary>
    /// 在庫設定期間の自動延長を利用するか
    /// </summary>
    public bool UseAutoExtend { get; set; }

    /// <summary>
    /// 在庫設定期間の毎月〇日の自動延長
    /// </summary>
    public int? AutoExtendEveryMonthDay { get; set; }

    /// <summary>
    /// 在庫設定期間を何カ月先まで延長するか？
    /// </summary>
    public int? AutoExtendMonth { get; set; }

    /// <summary>
    /// 利用可能期間開始
    /// </summary>
    public long? EnabledDateStart { get; set; }

    /// <summary>
    /// 利用可能期間終了
    /// </summary>
    public long? EnabledDateEnd { get; set; }

    /// <summary>
    /// 喫煙可能か
    /// </summary>
    public bool IsEnabledSmoking { get; set; }

    /// <summary>
    /// 情報JSON
    /// </summary>
    public string MetaJson { get; set; } = "{}";

    public RoomGroupMeta? Meta
    {
        get
        {
            try
            {
                return JsonConvert.DeserializeObject<RoomGroupMeta>(MetaJson);
            }
            catch
            {
                // 変換できない場合、新しいオブジェクトを返す
                return new RoomGroupMeta();
            }
        }
        init => MetaJson = JsonConvert.SerializeObject(value);
    }

    /// <summary>
    /// 部屋-カテゴリリレーション
    /// </summary>
    public ICollection<RoomGroupCategory>? RoomGroupCategories { get; set; }

    /// <summary>
    /// 部屋-ベッドタイプリリレーション
    /// </summary>
    public ICollection<RoomGroupBedType>? RoomGroupBedTypes { get; set; }

    /// <summary>
    /// プランー部屋ーキャンセル料リレーション
    /// </summary>
    public ICollection<PlanRoomGroupCancellation>? PlanRoomGroupCancellations { get; set; }

    /// <summary>
    /// 在庫自動調整ON/OFF
    /// </summary>
    public bool IsAutoExtend { get; set; }

    /// <summary>
    /// 毎月〇〇日の自動更新
    /// </summary>
    public int AutoExtendDayInMonth { get; set; }

    /// <summary>
    /// 〇〇ヵ月先まで自動更新
    /// </summary>
    public int AutoExtendAfterMonth { get; set; }

    /// <summary>
    /// ファイルリレーション
    /// </summary>
    public ICollection<FileRoomGroup>? FileRoomGroups { get; set; }

    /// <summary>
    /// 施設部屋リレーション
    /// </summary>
    public ICollection<FacilityRoomGroup>? FacilityRoomGroups { get; set; }

    /// <summary>
    /// プラン部屋リレーション
    /// </summary>
    public ICollection<PlanRoomGroup>? PlanRoomGroups { get; set; }

    /// <summary>
    /// プランサイト部屋日付リレーション
    /// </summary>
    public ICollection<PlanRoomGroupSiteAppDate>? PlanRoomGroupSiteAppDates { get; set; }

    /// <summary>
    /// サイト部屋日付リレーション
    /// </summary>
    public ICollection<RoomGroupSiteAppDate>? RoomGroupSiteAppDates { get; set; }

    /// <summary>
    /// プラン部屋サイト日別料金リレーション
    /// </summary>
    public ICollection<PlanRoomGroupSiteAppDatePriceData>? PlanRoomGroupSiteAppDatePriceData { get; set; }

    /// <summary>
    /// プラン部屋サイト割引リレーション
    /// </summary>
    public ICollection<PlanRoomGroupSiteDiscountData>? PlanRoomGroupSiteDiscountData { get; set; }

    /// <summary>
    /// 施設毎に年齢種別に対する設定をサイト毎にプランの部屋でどのように販売するかの設定
    /// </summary>
    public ICollection<PlanRoomGroupSitePersonAgeType>? PlanRoomGroupSitePersonAgeTypes { get; set; }

    public ICollection<RoomGroupAppDateTypePriceData>? RoomGroupAppDateTypePriceData { get; set; }

    public ICollection<PlanRoomGroupSiteAppDateTypePriceData>? PlanRoomGroupSiteAppDateTypePriceData { get; set; }

    public ICollection<RoomGroupSiteAppDateTypePriceData>? RoomGroupSiteAppDateTypePriceData { get; set; }

    /// <summary>
    /// 部屋-日付リレーション
    /// 在庫情報も含まれる
    /// </summary>
    public ICollection<RoomGroupAppDate>? RoomGroupAppDates { get; set; }

    public ICollection<PlanRoomGroupSite>? PlanRoomGroupSites { get; set; }

    public ICollection<RoomGroupSite>? RoomGroupSites { get; set; }

    /// <summary>
    /// 予約 プラン　・　部屋・日付リレーション
    /// </summary>
    public ICollection<ReservationPlanRoomGroupAppDate>? ReservationPlanRoomGroupAppDates { get; set; }

    public ICollection<ReservationRoomGroupAppDatePersonAgeType>? ReservationRoomGroupAppDatePersonAgeTypes
    {
        get;
        set;
    }

    public ICollection<ReservationRoomGroupAppDateOptionItem>? ReservationRoomGroupAppDateOptionItems { get; set; }

    public ICollection<RoomGroupSiteAppDatePriceData>? RoomGroupSiteAppDatePriceData { get; set; }

    public ICollection<RoomRoomGroup>? RoomRoomGroups { get; set; }
}
