using Liberty.Entity;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Newtonsoft.Json;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

/// <summary>
/// 宿泊プラン
/// </summary>
public class Plan : EntityData
{
    /// <summary>
    /// プラン名
    /// </summary>
    public string? Name { get; set; }

    public string? Description { get; set; }

    /// <summary>
    /// インポート用プラン名
    /// </summary>
    public string? NameForImport { get; set; }

    /// <summary>
    /// 利用可能期間開始
    /// </summary>
    [Obsolete("本機能はCanAcceptDateで代替えされているため不使用")]
    public long? EnabledDateStart { get; set; }

    /// <summary>
    /// 利用可能期間終了
    /// </summary>
    [Obsolete("本機能はCanAcceptDateで代替えされているため不使用")]

    public long? EnabledDateEnd { get; set; }

    /// <summary>
    /// 掲載期間設定を利用するか
    /// </summary>
    public bool UseDisplayDate { get; set; }

    /// <summary>
    /// 掲載期間開始
    /// </summary>
    public long? DisplayDateStart { get; set; }

    /// <summary>
    /// 掲載期間終了
    /// </summary>
    public long? DisplayDateEnd { get; set; }

    public bool CanDisplayDate(
        long appDateId
    )
    {
        // 使用しないのであればtrue
        if (!UseDisplayDate)
        {
            return true;
        }

        if (DisplayDateStart > appDateId)
        {
            return false;
        }

        return !(DisplayDateEnd < appDateId);
    }

    /// <summary>
    /// 受付期間を利用するか？
    /// </summary>
    public bool UseAcceptDate { get; set; }

    /// <summary>
    /// 受付期間開始
    /// </summary>
    public long? AcceptDateStart { get; set; }

    /// <summary>
    /// 受付期間終了
    /// </summary>
    public long? AcceptDateEnd { get; set; }

    public int? AcceptMonths { get; set; }

    public bool CanAcceptDate(
        long appDateId
    )
    {
        // 使用しないのであればtrue
        if (!UseAcceptDate)
        {
            return true;
        }

        if (AcceptDateStart > appDateId)
        {
            return false;
        }

        return !(AcceptDateEnd < appDateId);
    }

    /// <summary>
    /// 〇カ月先まで予約を受け取る
    /// </summary>
    public PlanAcceptEndLimitTypes AcceptEndLimitType { get; set; }

    /// <summary>
    /// 〇日先まで予約を受け取る
    /// </summary>
    public int? AcceptDays { get; set; }

    /// <summary>
    /// 日あたりの販売制限を利用するか
    /// </summary>
    public bool UseDaySaleLimit { get; set; }

    /// <summary>
    /// 日あたりの販売制限を利用するか
    /// </summary>
    public PlanDaySaleLimitTypes PlanDaySaleLimitType { get; set; }

    /// <summary>
    /// 組数で日あたりの販売制限
    /// </summary>
    public int? GroupNumberDaySaleLimit { get; set; }

    /// <summary>
    /// 部屋数で日あたりの販売制限
    /// </summary>
    public int? RoomNumberDaySaleLimit { get; set; }

    public bool CanDaySaleLimitReservedNumber(
        int reservedNumber
    )
    {
        // 使用しないのであればtrue
        if (!UseDaySaleLimit)
        {
            return true;
        }

        {
            // 組数で制限する場合
            if (PlanDaySaleLimitType != PlanDaySaleLimitTypes.RoomGroup || RoomNumberDaySaleLimit is null)
            {
                return true;
            }

            if (RoomNumberDaySaleLimit <= reservedNumber)
            {
                return false;
            }
        }

        return true;
    }

    public bool CanDaySaleLimitReservedPairs(
        int reservationPairs
    )
    {
        // 使用しないのであればtrue
        if (!UseDaySaleLimit)
        {
            return true;
        }

        {
            // 組数で制限する場合
            if (PlanDaySaleLimitType != PlanDaySaleLimitTypes.Pair || GroupNumberDaySaleLimit is null)
            {
                return true;
            }

            if (GroupNumberDaySaleLimit <= reservationPairs)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 人数制限を利用するか
    /// </summary>
    public bool UseAcceptPersonNumber { get; set; }

    /// <summary>
    /// 人数制限最小
    /// </summary>
    public int? AcceptPersonNumberMin { get; set; }

    /// <summary>
    /// 人数制限最大
    /// </summary>
    public int? AcceptPersonNumberMax { get; set; }

    public bool CanDaySaleLimitAllPersons(
        int allPersons
    )
    {
        // 使用しないのであればtrue
        if (!UseAcceptPersonNumber)
        {
            return true;
        }

        // 受け入れ人数最大
        if (AcceptPersonNumberMax < allPersons)
        {
            return false;
        }

        // 受け入れ人数最小
        return !(AcceptPersonNumberMin > allPersons);
    }

    /// <summary>
    /// 受け入れ可能泊数最小
    /// </summary>
    public int? NumberOfStayLimitMin { get; set; }

    /// <summary>
    /// 受け入れ可能泊数最大
    /// </summary>
    public int? NumberOfStayLimitMax { get; set; }

    public bool CanNumberOfStayLimitMax(
        int restNumber
    )
    {
        if (NumberOfStayLimitMax < restNumber)
        {
            return false;
        }

        return !(NumberOfStayLimitMin > restNumber);
    }

    /// <summary>
    /// チェックイン開始時刻
    /// nullの場合は宿のデフォルトを使用
    /// </summary>
    public TimeSpan? CheckInStart { get; set; }

    /// <summary>
    /// チェックイン終了時刻
    /// nullの場合は宿のデフォルトを使用
    /// </summary>
    public TimeSpan? CheckInEnd { get; set; }

    /// <summary>
    /// チェックアウト時刻
    /// nullの場合は宿のデフォルトを使用
    /// </summary>
    public TimeSpan? CheckOut { get; set; }

    /// <summary>
    /// 予約受付日　何日前まで受け付けるか
    /// </summary>
    public int? ReceptionDayLimit { get; set; }

    /// <summary>
    /// 宿泊受付期限
    /// </summary>
    public TimeSpan? ReceptionLimit { get; set; }

    /// <summary>
    /// キャンセル期限種別は予約受付・予約変更締め切り時間と同じか？
    /// </summary>
    public bool IsCancelSameAccept { get; set; }

    /// <summary>
    /// キャンセル期限種別
    /// </summary>
    public int? CancelDayLimit { get; set; }

    /// <summary>
    /// キャンセル期限
    /// </summary>
    public TimeSpan? CancelLimit { get; set; }

    ///// <summary>
    ///// キャンセル規定ID
    ///// </summary>
    //public long CancellationID { get; set; }

    /// <summary>
    /// キャンセル規定
    /// </summary>
    public Cancellation? Cancellation { get; set; }

    /// <summary>
    /// 現地決済か？
    /// </summary>
    public bool IsOnSidePayment { get; set; }

    /// <summary>
    /// オンライン決済か？
    /// </summary>
    public bool IsOnLinePayment { get; set; }

    /// <summary>
    /// タグ
    /// </summary>
    public string? Tag { get; set; }

    /// <summary>
    /// URLスラッグ
    /// </summary>
    public string? Slag { get; set; }

    public bool IsSecret { get; set; }

    /// <summary>
    /// シークレットワード
    /// </summary>
    public string? SecretWord { get; set; }

    /// <summary>
    /// 固定オプションを使用するか？
    /// </summary>
    public bool UseFixedOptionItem { get; set; }

    /// <summary>
    /// 任意オプションを使用するか？
    /// </summary>
    public bool UseOptionalOptionItem { get; set; }

    /// <summary>
    /// ポイント付与率リレーション用ID
    /// </summary>
    public long? PointRateId { get; set; }

    /// <summary>
    /// ポイント付与率
    /// </summary>
    public PointRate? PointRate { get; set; }

    /// <summary>
    /// 施設プランリレーション
    /// </summary>
    public ICollection<FacilityPlan>? FacilityPlans { get; set; }

    /// <summary>
    /// プラン サイト　リレーション
    /// </summary>
    public ICollection<PlanSite>? PlanSites { get; set; }

    /// <summary>
    /// 対象部屋タイプリレーション
    /// </summary>
    public ICollection<PlanRoomGroup>? PlanRoomGroups { get; set; }

    /// <summary>
    /// プランカテゴリリレーション
    /// </summary>
    public ICollection<PlanCategory>? PlanCategories { get; set; }

    /// <summary>
    /// ファイルプランリレーション
    /// </summary>
    public ICollection<FilePlan>? FilePlans { get; set; }

    /// <summary>
    /// 【固定/任意オプション】
    /// プラン　オプションアイテムリレーション
    /// </summary>
    public ICollection<PlanOptionItem>? PlanOptionItems { get; set; }

    /// <summary>
    /// 【固定/任意オプション】
    /// プラン　オプションアイテムリレーション
    /// </summary>
    public List<PlanOptionItem>? GetFixedOptionItems()
    {
        return PlanOptionItems?
            .Where(a => a.PlanOptionItemType == PlanOptionItemTypes.Fixed)
            .ToList();
    }

    /// <summary>
    /// 【固定/任意オプション】
    /// プラン　オプションアイテムリレーション
    /// </summary>
    public List<PlanOptionItem>? GetOptionalOptionItems()
    {
        return PlanOptionItems?
            .Where(a => a.PlanOptionItemType == PlanOptionItemTypes.Optional)
            .ToList();
    }

    /// <summary>
    /// 情報JSON
    /// </summary>
    public string MetaJson { get; set; } = "{}";

    public PlanMeta? Meta
    {
        get
        {
            try
            {
                return JsonConvert.DeserializeObject<PlanMeta>(MetaJson);
            }
            catch
            {
                // 変換できない場合、新しいオブジェクトを返す
                return new PlanMeta();
            }
        }
        set => MetaJson = JsonConvert.SerializeObject(value);
    }

    /// <summary>
    /// 予約者への質問リレーション
    /// </summary>
    public ICollection<PlanQuestion>? PlanQuestions { get; set; }

    /// <summary>
    /// プランサイト部屋日付リレーション
    /// </summary>
    public ICollection<PlanRoomGroupSiteAppDate>? PlanRoomGroupSiteAppDates { get; set; }

    /// <summary>
    /// プラン部屋サイトの基準料金・・・分割数も担う
    /// プラン部屋サイト料金タイプ×人数分割に対する料金設定
    /// </summary>
    public ICollection<PlanRoomGroupSiteAppDateTypePriceData>? PlanRoomGroupSiteAppDateTypePriceData { get; set; }

    /// <summary>
    /// プラン部屋サイト日別料金リレーション
    /// 実質この料金が販売価格となる
    /// PlanSitePriceDataや
    /// </summary>
    public ICollection<PlanRoomGroupSiteAppDatePriceData>? PlanRoomGroupSiteAppDatePriceData { get; set; }

    /// <summary>
    /// 施設毎に年齢種別に対する設定をサイト毎にプランの部屋でどのように販売するかの設定
    /// </summary>
    public ICollection<PlanRoomGroupSitePersonAgeType>? PlanRoomGroupSitePersonAgeTypes { get; set; }

    public ICollection<PlanRoomGroupSite>? PlanRoomGroupSites { get; set; }

    public ICollection<PlanMealType>? PlanMealTypes { get; set; }

    /// <summary>
    /// プランー部屋ーキャンセル料リレーション
    /// </summary>
    public ICollection<PlanRoomGroupCancellation>? PlanRoomGroupCancellations { get; set; }

    /// <summary>
    /// 予約 プラン　・　部屋・日付リレーション
    /// </summary>
    public ICollection<ReservationPlanRoomGroupAppDate>? ReservationPlanRoomGroupAppDates { get; set; }

    /// <summary>
    /// プラン部屋サイト割引リレーション
    /// </summary>
    public ICollection<PlanRoomGroupSiteDiscountData>? PlanRoomGroupSiteDiscountData { get; set; }

    /// <summary>
    /// いずれかの支払い方法が設定されているか？
    /// SQLを組む際は本プロパティは使用できないので、実装内容を指定すること
    /// </summary>
    public bool HasPayment => IsOnSidePayment || IsOnLinePayment;

    /// <summary>対象の部屋タイプがこのプランで販売可能か？</summary>
    /// <param name="roomGroupId"></param>
    /// <param name="siteId"></param>
    /// <returns></returns>
    public bool CanBeSelledRoomGroup(
        long roomGroupId,
        long siteId
    )
    {
        return PlanRoomGroupSitePersonAgeTypes?
            .Where(a => a.RoomGroupId == roomGroupId)
            .Any(a => a.SiteId == siteId) ?? false;
    }
}
