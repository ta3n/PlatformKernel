namespace PlatformKernel.Service.Base.Application.Constants;

public enum QueTypes
{
    Unknown,
    Mail
}

public enum Genders
{
    None,
    Male,
    Female
}

public enum PointTypes
{
    /// <summary>ポイント獲得</summary>
    Income,

    /// <summary>ポイント支出</summary>
    Spend,

    /// <summary>期限切れ</summary>
    Expire,

    /// <summary>キャンセル</summary>
    Cancelled
}

/// <summary>ポイント取引種別</summary>
public enum PointRateTypes
{
    /// <summary>不明</summary>
    Unknown = 0,

    /// <summary>予約時</summary>
    Reservation = 1
}

/// <summary>
/// カテゴリ分類
/// ex:施設分類、部屋分類等それぞれの情報に「論理的な分類」を設定し、パッケージBの抽出に使用します
/// </summary>
public enum CategoryTypes
{
    /// <summary>1:施設</summary>
    Facility = 1 << 1,

    /// <summary>2:施設特徴</summary>
    FacilityFeature = 1 << 2,

    /// <summary>3:施設設備</summary>
    FacilityEquipment = 1 << 3,

    /// <summary>4:部屋</summary>
    RoomGroup = 1 << 4,

    /// <summary>5:部屋特徴</summary>
    RoomGroupFeature = 1 << 5,

    /// <summary>6:部屋設備</summary>
    RoomGroupEquipment = 1 << 6,

    /// <summary>7:周辺・レジャー</summary>
    Leisure = 1 << 7,

    /// <summary>8:お風呂・温泉</summary>
    Spa = 1 << 8,

    /// <summary>9:景観</summary>
    View = 1 << 9,

    /// <summary>10:プラン</summary>
    Plan = 1 << 10,

    /// <summary>11:アメニティ</summary>
    Amenity = 1 << 11,

    /// <summary>12:オプション</summary>
    OptionItem = 1 << 12,

    /// <summary>13:ファイル・メディア</summary>
    File = 1 << 13,

    /// <summary>14:食事種別</summary>
    MealType = 1 << 14
}

/// <summary>
/// ファイルの仕様用途
/// </summary>
[Flags]
public enum FilePurposeTypes
{
    None = 0,

    /// <summary>施設外観</summary>
    FacilityAppearance = 1,

    /// <summary>施設料理</summary>
    FacilityMeal = 1 << 1,

    /// <summary>施設部屋</summary>
    FacilityRoomGroup = 1 << 2,

    /// <summary>施設お風呂</summary>
    FacilitySpa = 1 << 3,

    /// <summary>施設館内</summary>
    FacilityHall = 1 << 4,

    /// <summary>施設周辺</summary>
    FacilityAround = 1 << 5,

    /// <summary>施設その他</summary>
    FacilityOther = 1 << 6,

    /// <summary>施設ロゴ</summary>
    FacilityLogo = 1 << 7
}

public enum PaymentTypes
{
    Unknown,
    OnSidePayment,
    OnLinePayment
}

/// <summary>
/// 組・部屋数・宿泊人数に対して
/// </summary>
public enum OptionItemTargets
{
    Unknown,

    /// <summary>組に対して</summary>
    Pair,

    /// <summary>部屋数に対して</summary>
    Room,

    /// <summary>宿泊人数に対して</summary>
    PersonNumber
}

/// <summary>
/// プランオプション:固定オプション、任意オプション
/// </summary>
public enum PlanOptionItemTypes
{
    /// <summary>固定オプション</summary>
    Fixed,

    /// <summary>任意オプション</summary>
    Optional
}

/// <summary>
/// プラン受付制限最大日をどのように決めるか
/// </summary>
public enum PlanAcceptEndLimitTypes
{
    /// <summary>〇カ月先まで予約を受け取る</summary>
    AfterMonths,

    /// <summary>〇日先まで予約を受け取る</summary>
    AfterDays
}

/// <summary>
/// プランを一日当たり〇組or〇室で締め切るか
/// </summary>
public enum PlanDaySaleLimitTypes
{
    /// <summary>〇部屋まで予約を受け付ける</summary>
    RoomGroup,

    /// <summary>〇組まで予約を受け取る</summary>
    Pair
}

public enum PriceSettingTypes
{
    /// <summary>何もしない：大人料金として扱う</summary>
    None,

    /// <summary>パーセント</summary>
    Percent,

    /// <summary>固定</summary>
    Price,

    /// <summary>値引き</summary>
    Discount
}

public enum ReservationStatus
{
    /// <summary>
    /// 一時的に保存したが、確定されない
    /// </summary>
    Temporary = 1 << 0,

    /// <summary>
    /// ロクインしないゲスト予約実装が完了する
    /// </summary>
    Confirmed = 1 << 1,

    /// <summary>
    /// ロクインしたユーザー予約実装が完了する
    /// </summary>
    Reserved = 1 << 2,

    /// <summary>
    /// ＊ログインした一般ユーザがが予約を キャンセル する
    /// </summary>
    UserCanceled = 1 << 3,

    /// <summary>
    /// ログインしないゲストユーザが予約を キャンセル する
    /// </summary>
    GuestCanceled = 1 << 4,

    /// <summary>
    /// ＊施設管理者が予約を キャンセルする
    /// </summary>
    ManagerCanceled = 1 << 5,

    /// <summary>
    /// 新しくModifiedのレコードが生成されて、そちらに変更後のデーターが書き込めため、変更前のデーターとして、参照用（ReadOnly）のデーターとなったもの
    /// </summary>
    UserModified = 1 << 6,

    /// <summary>
    /// 新しくModifiedのレコードが生成されて、そちらに変更後のデーターが書き込めため、変更前のデーターとして、参照用（ReadOnly）のデーターとなったもの
    /// </summary>
    ManagerModified = 1 << 7,

    /// <summary>
    /// ログインしないゲストユーザが予約を変更します
    /// </summary>
    GuestModified = 1 << 8,

    /// <summary>
    /// 変更された新しい予約の状態です。
    /// ＊ログインした一般ユーザがが予約を変更する
    /// ＊施設管理者が予約を変更する
    /// </summary>
    Modified = 1 << 9,

    /// <summary>
    /// 決済やゲストの予約認証ができましたが、そのタイミングで予約が確定できなかった場合、このステータスになります。
    /// </summary>
    Failed = 1 << 10
}

public enum ReservationQuestionTypes
{
    /// <summary>不明</summary>
    Unknown,

    /// <summary>プラン</summary>
    Plan,

    /// <summary>オプションについての質問</summary>
    OptionItem
}

public enum RoomGroupSizeUnitTypes
{
    /// <summary畳</summary>
    JYO,

    /// <summary平米</summary>
    M2
}

public enum BedTypeUnitTypes
{
    /// <summary組み</summary>
    Pair
}

public enum QuestionTypes
{
    Unknown,
    Text,
    Textarea,
    Checkbox,
    Radio,
    Select
}

/// <summary>
/// 予約操作種別
/// 予約情報をどのアクタがおこなったか
/// </summary>
public enum ReservationOperationTypes
{
    /// <summary>不明・指定なし</summary>
    Unknown,

    /// <summary>オンライン決済未承認時一定時間経過による予約キャンセル</summary>
    OnlinePaymentNoApproved,

    /// <summary>宿による予約キャンセル</summary>
    Facility,

    /// <summary>一般ユーザによる予約キャンセル</summary>
    User
}

public enum MealTypeEatTypes
{
    /// <summary>不明・指定なし</summary>
    Unknown,

    /// <summary個室</summary>
    Box,

    /// <summary部屋食</summary>
    Room
}

/// <summary>
/// 管理画面操作モード
/// </summary>
public enum OperationModes
{
    /// <summary>不明・指定なし</summary>
    Unknown,

    /// <summaryイージー</summary>
    Easy,

    /// <summaryアドバンス</summary>
    Room
}

[Flags]
public enum FoodBeds
{
    /// <summary>なし</summary>
    None = 0,

    /// <summary>食事あり</summary>
    Food = 1,

    /// <summary>布団あり</summary>
    Bed = 1 << 1
}

/// <summary>
/// PersonAgeTypeに対して、あすなろで扱う
/// 「大人(mon/womon)」
/// 「子供(小学生高学年teen/小学生低学年teen_b)」
/// 「幼児(child)」
/// 「乳児(baby)」
/// </summary>
[Flags]
public enum PersonAgeGroups
{
    /// <summary>不明</summary>
    None = 0,

    /// <summary>大人</summary>
    Adult = 1,

    /// <summary>子供(高学年)</summary>
    Teen = 1 << 1,

    /// <summary>小学生(低学年)</summary>
    TeenB = 1 << 2,

    /// <summary>乳児</summary>
    Child = 1 << 3,

    /// <summary>乳児</summary>
    Baby = 1 << 4
}

public static class Culture
{
    public static readonly System.Globalization.CultureInfo SystemCulture = new("en-US");
}

public enum GmoPaymentResultRequestStatus
{
    UNPROCESSED,
    AUTHENTICATED,
    CHECK,
    CAPTURE,
    AUTH,
    SALES,
    VOID,
    RETURN,
    RETURNX,
    SAUTH
}

public enum PlanTypes
{
    Combo = 1 << 0,
    RoomOnly = 1 << 1
}

public enum BookingSendMailState
{
    ReminderOfUpcomingCheckInDateSendMail,
    ReminderOfUpcomingCheckInDateSentMail,
    CancellationFeeReminderSendMail,
    CancellationFeeReminderSentMail
}

public enum UserBookingFilter
{
    ReservationCompleted = 1,
    Canceled = 2
}

public enum CancellationFeeType
{
    UserInput,
    SystemDefined
}

public enum CancellationStatus
{
    NotCancelled,
    CancelledRefunded,
    CancelledPaymentIssue,
    CancelledDepositRefunded,
    CancelledLocalPaymentRefunded,
    CancelledLocalFinalized
}

public enum MailActorTypes
{
    Site,
    Manager,
    Employee,
    User,
    Guest,
    Reminder
}

public enum MailSentStatus
{
    Success = 1,
    Failed = 2
}

public enum ItemTypes
{
    None,
    Folder,
    Tab,
    Field
}
