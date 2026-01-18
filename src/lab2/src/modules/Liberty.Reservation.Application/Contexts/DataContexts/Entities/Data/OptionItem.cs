using Liberty.Entity;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

/// <summary>
/// 別注料理等オプション商品
/// </summary>
public class OptionItem : EntityData
{
    /// <summary>
    /// 名前
    /// </summary>
    public MultilingualText? Name { get; set; }

    /// <summary>
    /// 単価
    /// </summary>
    public int? Price { get; set; }

    /// <summary>
    /// 説明
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 基本提供数
    /// </summary>
    public int? BaseNumber { get; set; }

    public int? MaxSupplyNumber { get; set; }

    /// <summary>
    /// キャンセル料が発生するオプションか？
    /// </summary>
    public bool UseCancelFee { get; set; }

    /// <summary>
    /// 運用開始日時
    /// </summary>
    public DateTime? EnabledStart { get; set; }

    /// <summary>
    /// 運用終了日時
    /// </summary>
    public DateTime? EnabledEnd { get; set; }

    /// <summary>
    /// 受付締め切り日時
    /// </summary>
    public bool UseOrderLimit { get; set; }

    /// <summary>
    /// 宿泊日の〇日前
    /// </summary>
    public int? OrderLimitBeforeDay { get; set; }

    /// <summary>
    /// 〇日前の〇〇時分
    /// </summary>
    public TimeSpan? OrderLimitBeforeDaySpan { get; set; }

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
    /// 販売除外日設定を使用するか？
    /// </summary>
    public bool UseNotSelled { get; set; }

    /// <summary>
    /// 利用可能期間開始
    /// </summary>
    public DateTime? EnabledDateStart { get; set; }

    /// <summary>
    /// 利用可能期間終了
    /// </summary>
    public DateTime? EnabledDateEnd { get; set; }

    /// <summary>
    /// 掲載期間設定
    /// </summary>
    public bool UseDisplayDate { get; set; }

    /// <summary>
    /// 掲載期間開始
    /// </summary>
    public DateTime? DisplayDateStart { get; set; }

    /// <summary>
    /// 掲載期間終了
    /// </summary>
    public DateTime? DisplayDateEnd { get; set; }

    /// <summary>
    /// 受付期間設定
    /// </summary>
    public bool UseAcceptDate { get; set; }

    /// <summary>
    /// 受付期間開始
    /// </summary>
    public DateTime? AcceptDateStart { get; set; }

    /// <summary>
    /// 受付期間終了
    /// </summary>
    public DateTime? AcceptDateEnd { get; set; }

    /// <summary>
    /// 施設オプションリレーション
    /// </summary>
    public ICollection<FacilityOptionItem>? FacilityOptionItems { get; set; }

    /// <summary>
    /// オプションカテゴリリレーション
    /// </summary>
    public ICollection<OptionItemCategory>? OptionItemCategories { get; set; }

    /// <summary>
    /// オプションアイテム質問リレーション
    /// </summary>
    public ICollection<OptionItemQuestion>? OptionItemQuestions { get; set; }

    /// <summary>
    /// オプションアイテム-画像リレーション
    /// </summary>
    public ICollection<FileOptionItem>? FileOptionItems { get; set; }

    /// <summary>
    /// プラン　オプションアイテムリレーション
    /// </summary>
    public ICollection<PlanOptionItem>? PlanOptionItems { get; set; }

    /// <summary>
    /// オプションアイテム-日付リレーション
    /// 在庫情報も含まれる
    /// </summary>
    public ICollection<OptionItemAppDate>? OptionItemAppDates { get; set; }

    public ICollection<ReservationRoomGroupAppDateOptionItem>? ReservationRoomGroupAppDateOptionItems { get; set; }
}
