using Liberty.Entity;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

/// <summary>
/// 施設
/// </summary>
public class Facility : EntityData
{
    public MultilingualText? Name { get; set; }

    public string? Kana { get; set; }

    public MultilingualText? Address1 { get; set; }
    public MultilingualText? Address2 { get; set; }
    public MultilingualText? Address3 { get; set; }
    public MultilingualText? Address4 { get; set; }

    /// <summary>
    /// アクセス概要、説明
    /// </summary>
    public MultilingualText? AccessInfoComment { get; set; }

    /// <summary>
    /// 最寄り駅
    /// </summary>
    public MultilingualText? NearStationInfoComment { get; set; }

    /// <summary>
    /// 送迎補足
    /// </summary>
    public MultilingualText? TransferComment { get; set; }

    /// <summary>
    /// 駐車場補足
    /// </summary>
    public MultilingualText? ParkingInfoComment { get; set; }

    public string? Postcode { get; set; }

    /// <summary>
    /// 説明
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// タグ
    /// </summary>
    public string? Tag { get; set; }

    public long? AreaId { get; set; }

    /// <summary>
    /// メインエリア
    /// </summary>
    public Area? Area { get; set; }

    public long? CategoryId { get; set; }

    /// <summary>
    /// メインカテゴリ
    /// </summary>
    public Category? Category { get; set; }

    /// <summary>
    /// Tel
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Fax
    /// </summary>
    public string? Fax { get; set; }

    public bool UseFax { get; set; }

    /// <summary>
    /// Url
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// 緯度
    /// </summary>
    public float Latitude { get; set; }

    /// <summary>
    /// 経度
    /// </summary>
    public float Longitude { get; set; }

    /// <summary>
    /// 入湯税自動計算使用有無
    /// </summary>
    public bool UseSpaTaxAutoCalc { get; set; }

    /// <summary>
    /// 現地決済か？
    /// </summary>
    public bool IsOnSidePayment { get; set; }

    /// <summary>
    /// オンライン決済を使用するか？
    /// </summary>
    public bool IsOnLinePayment { get; set; }

    /// <summary>
    /// オンライン決済仕様可能か？(宿泊管理側の機能)
    /// </summary>
    public bool CanOnLinePayment { get; set; }

    /// <summary>
    /// キャンセル期限（時）
    /// </summary>
    public int CancelLimitHour { get; set; }

    /// <summary>
    /// キャンセル期限（分）
    /// </summary>
    public int CancelLimitMinute { get; set; }

    public FacilityMeta? Meta { get; set; }

    /// <summary>
    /// 備考
    /// </summary>
    public string? Memo { get; set; }

    public int? MinimumPrice { get; set; }

    public bool IsEnabledMinimumPrice { get; set; } = false;

    public FaxService? FaxService =>
        FacilityFaxServices?
            .Where(a => a.IsEnabled)
            .Select(a => a.FaxService)
            .FirstOrDefault();

    public bool IsFaxServiceEnabled => FaxService is not null;

    [ForeignKey("Parent")]
    public long? ParentId { set; get; }

    public Facility? Parent { set; get; }

    /// <summary>支店展開用</summary>
    public ICollection<Facility>? Children { get; set; }

    /// <summary>
    /// 施設ーキャンセル規定詳細リレーション
    /// </summary>
    public ICollection<FacilityCancellation>? FacilityCancellations { get; set; }

    /// <summary>
    /// 施設・入湯税期間グループリレーション
    /// </summary>
    public ICollection<FacilitySpaTaxGroup>? FacilitySpaTaxGroups { get; set; }

    /// <summary>
    /// アレルゲン対応
    /// </summary>
    public ICollection<FacilityAllergen>? FacilityAllergens { get; set; }

    /// <summary>
    /// 掲載先サイトリレーション
    /// </summary>
    public ICollection<FacilitySite>? FacilitySites { get; set; }

    /// <summary>
    /// 施設カレンダーリレーション
    /// </summary>
    public ICollection<FacilityCalendar>? FacilityCalendars { get; set; }

    public ICollection<FacilityAppDateType>? FacilityAppDateTypes { get; set; }

    /// <summary>
    /// 施設部屋リレーション
    /// </summary>
    public ICollection<FacilityRoomGroup>? FacilityRoomGroups { get; set; }

    /// <summary>
    /// 施設プランリレーション
    /// </summary>
    public ICollection<FacilityPlan>? FacilityPlans { get; set; }

    /// <summary>
    /// 施設・年齢種別設定
    /// </summary>
    public ICollection<FacilityPersonAgeType>? FacilityPersonAgeTypes { get; set; }

    /// <summary>
    /// 施設ファックスサービスリレーション
    /// </summary>
    public ICollection<FacilityFaxService>? FacilityFaxServices { get; set; }

    /// <summary>
    /// 施設メディアリレーション
    /// </summary>
    public ICollection<FacilityFile>? FacilityFiles { get; set; }

    /// <summary>
    /// 施設オプションリレーション
    /// </summary>
    public ICollection<FacilityOptionItem>? FacilityOptionItems { get; set; }

    /// <summary>
    /// 施設質問リレーション
    /// </summary>
    public ICollection<FacilityQuestion>? FacilityQuestions { get; set; }

    /// <summary>
    /// 施設が属するカテゴリ
    /// </summary>
    public ICollection<FacilityCategory>? FacilityCategories { get; set; }

    /// <summary>
    /// 現地決済か？
    /// </summary>
    public bool UseDailyPerson { get; set; }

    public bool CanAddRoomOnModify { get; set; }

    /// <summary>
    /// 見出し1
    /// </summary>
    public MultilingualText? Heading1 { get; set; }

    /// <summary>
    /// 入湯税に関する補足
    /// </summary>
    public MultilingualText? SpaTaxComment { get; set; }

    public MultilingualText? SpaTaxTable { get; set; }

    /// <summary>
    /// バリアフリー補足
    /// </summary>
    public MultilingualText? BarrierFreeInfoComment { get; set; }
}
