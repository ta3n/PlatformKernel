using System.ComponentModel.DataAnnotations.Schema;
using Liberty.Entity;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

/// <summary>
/// 予約情報
/// </summary>
public class Reservation : EntityData
{
    public string? Serial { get; set; }

    public long FacilityId { get; set; }
    public string? FacilityRecordCode { get; set; }
    public Facility? Facility { get; set; }

    public long SiteId { get; set; }
    public Site? Site { get; set; }

    public long PlanId { get; set; }
    public Plan? Plan { get; set; }

    public long RoomGroupId { get; set; }
    public RoomGroup? RoomGroup { get; set; }

    public string? UserCode { get; set; }

    /// <summary>
    /// 登録済みの予約者がいるか？
    /// </summary>
    public bool HasApplicationUser => !string.IsNullOrEmpty(UserCode);

    /// <summary>
    /// 予約者情報
    /// </summary>
    public CustomerInfo? Reserver { get; set; }

    public long ReserverId { get; set; }

    /// <summary>
    /// 宿泊代表者
    /// </summary>
    public CustomerInfo? MainUser { get; set; }

    public long? MainUserId { get; set; }

    public long CheckInDate { get; set; }

    public long CheckOutDate
    {
        get
        {
            var checkOutDate = AppDate.GetDateTime(CheckInDate).AddDays(RestNumber);
            return AppDate.GetId(checkOutDate);
        }
    }

    public int RestNumber { get; set; }

    public int RoomNumber { get; set; }

    public TimeSpan? CheckInTime { get; set; }

    public TimeSpan? CheckOutTime { get; set; }

    public PaymentTypes PaymentType { get; set; }

    public int UsedPoint { get; set; }

    public int UpdateCount { get; set; }

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

    public ICollection<ReservationPoint>? ReservationPoints { get; set; }

    /// <summary>
    /// 予約・質問回答リレーション
    /// </summary>
    public ICollection<ReservationQuestion>? ReservationQuestions { get; set; }

    /// <summary>
    /// 注文情報とのリレーション
    /// </summary>
    public ICollection<OrderReservation>? OrderReservations { get; set; }

    public GmoPaymentResultRequest? GmoPaymentResultRequest
    {
        get
        {
            var orderReservation = OrderReservations?.FirstOrDefault();
            var orderGmoPaymentResultRequest = orderReservation?
                .Order?.OrderGmoPaymentResultRequests?.FirstOrDefault();

            return orderGmoPaymentResultRequest?.GmoPaymentResultRequest;
        }
    }

    public bool IsOnSidePayment => PaymentType == PaymentTypes.OnSidePayment;

    public bool IsOnlinePayment => PaymentType == PaymentTypes.OnLinePayment;

    /// <summary>
    /// GMOペイメントのリクエストリスト
    /// </summary>
    public ICollection<OrderGmoPaymentResultRequest>? OrderGmoPaymentResultRequests()
    {
        return OrderReservations?
            .Select(x => x.Order)
            .SelectMany(x => x?.OrderGmoPaymentResultRequests ?? [])
            .ToList();
    }

    /// <summary>GMOペイメントに取消があるか？</summary>
    public bool HasGmoPaymentVoid()
    {
        return OrderGmoPaymentResultRequests()
                ?.Any(
                    x => x.GmoPaymentResultRequest?.IsVoid ?? false
                )
            ?? false;
    }

    /// <summary>GMOペイメントに返品があるか？</summary>
    public bool HasGmoPaymentReturn()
    {
        return OrderGmoPaymentResultRequests()
                ?.Any(
                    x => x.GmoPaymentResultRequest?.IsReturn ?? false
                )
            ?? false;
    }

    /// <summary>GMOペイメントに月跨り返品があるか？</summary>
    public bool HasGmoPaymentReturnX()
    {
        return OrderGmoPaymentResultRequests()
                ?.Any(
                    x => x.GmoPaymentResultRequest?.IsReturnX ?? false
                )
            ?? false;
    }

    /// <summary>
    /// GMOペイメントの支払い処理がすべて完了しているか？
    /// </summary>
    public bool IsGmoPaymentCompleteAll()
    {
        // 取り消し処理があるか？
        if (HasGmoPaymentVoid())
        {
            return false;
        }

        // 返品処理があるか？
        if (HasGmoPaymentReturn())
        {
            return false;
        }

        // 月跨り返品があるか？
        if (HasGmoPaymentReturnX())
        {
            return false;
        }

        var amount = Amount;
        var tax = SpaTax;

        // 決済完了している支払いで抽出
        var gmoPaymentResultRequests = OrderGmoPaymentResultRequests()
            ?.Where(
                x => x.GmoPaymentResultRequest is
                {
                    IsCapture: true,
                    IsCompleted: true
                }
            )
            .ToList();

        var sAmount = gmoPaymentResultRequests?.Sum(
                a => long.Parse(a.GmoPaymentResultRequest?.Amount ?? "0")
            )
            ?? 0;
        var sTax = gmoPaymentResultRequests?.Sum(
                a => long.Parse(a.GmoPaymentResultRequest?.Tax ?? "0")
            )
            ?? 0;

        // 完了しているGMOペイメントの通知の合計金額が同じであれば、支払い完了とする
        if (amount != sAmount)
        {
            return false;
        }

        if (tax != sTax)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 支払い金額
    /// </summary>
    public decimal? Amount => BookingData?.AppDates?.Sum(x => x.TotalPrice);

    /// <summary>
    /// 入湯税
    /// </summary>
    public decimal? SpaTax => BookingData?.AppDates?.Sum(x => x.SpaTax);

    /// <summary>
    /// 獲得予定ポイント
    /// </summary>
    public int IncomePoint { get; set; }

    /// <summary>
    /// 予約日
    /// </summary>
    public DateTime ReservationDateTime { get; set; }

    /// <summary>
    /// 承認時刻
    /// </summary>
    public DateTime? ConfirmedDateTime { get; set; }

    /// <summary>
    /// 状態
    /// </summary>
    public ReservationStatus ReservationState { get; set; } = ReservationStatus.Temporary;

    public DateTime? ModifiedDateTime { get; set; }
    public DateTime? CancelledDateTime { get; set; }
    public decimal? CancellationPrice { get; set; }
    public float? CancelRateFee { get; set; }
    public CancellationFeeType? CancellationFeeType { get; set; }
    public CancellationStatus? CancellationStatus { get; set; }

    public DateTime? NoShowDateTime { get; set; }
    public string? NoShowReason { get; set; }

    public bool IsNoShow { get; set; }

    [ForeignKey("Parent")]
    public long? ParentId { set; get; }

    public Reservation? Parent { set; get; }

    /// <summary>
    /// 備考
    /// </summary>
    public string? Memo { get; set; }

    /// <summary>
    /// 承認済みか？
    /// </summary>
    public bool IsUnConfirmed => ReservationState is ReservationStatus.Temporary;

    /// <summary>
    /// 予約成立状態か？
    /// </summary>
    public bool IsReserved => ReservationState is ReservationStatus.Reserved or ReservationStatus.Confirmed or ReservationStatus.Modified
        && !IsNoShow;

    /// <summary>
    /// 予約キャンセル状態か？
    /// </summary>
    public bool IsCancelled
        => ReservationState is ReservationStatus.UserCanceled or ReservationStatus.ManagerCanceled or ReservationStatus.GuestCanceled;

    /// <summary>
    /// 予約変更済みの状態か？
    /// </summary>
    public bool IsModified
        => ReservationState is ReservationStatus.UserModified or ReservationStatus.ManagerModified or ReservationStatus.GuestModified;

    /// <summary>
    /// 情報JSON
    /// </summary>
    public BookingData? BookingData { get; set; }

    public bool IsSameMainUser { get; set; }
    public bool UseRoomUser { get; set; }

    /// <summary>
    /// キャンセル料対象料金取得
    /// </summary>
    public decimal CancellationTargetPrice
    {
        get
        {
            // 計算 宿泊料金、オプション料金が対象
            decimal totalPrice = 0;
            totalPrice += BookingData?.TotalRoomPrice ?? 0;
            totalPrice += BookingData?.TotalOptionPrice ?? 0;

            return totalPrice;
        }
    }

    public bool CanModifyByUser(
        bool isCanCancel = true
    )
    {
        var checkInDate = AppDate.GetDateTime(CheckInDate, CheckInTime);
        return CanModifyInternal(checkInDate, isCanCancel);
    }

    public bool CanModifyByManager(
        bool isCanCancel = true
    )
    {
        var checkInDate = AppDate.GetDateTime(CheckInDate);
        return CanModifyInternal(checkInDate, isCanCancel);
    }

    private bool CanModifyInternal(
        DateTime checkInDate,
        bool isCanCancel
    )
    {
        // 以下の条件の場合変更・キャンセル不可
        if (!IsReserved)
        {
            return false;
        }

        var now = DateTime.UtcNow.AddHours(DefaultValues.TimeZoneOffset);

        var plan = Plan;
        if (plan is null)
        {
            return false;
        }

        var cancelDayLimit = plan.ReceptionDayLimit;
        var cancelLimit = plan.ReceptionLimit;
        var isCancelSameAccept = !plan.IsCancelSameAccept;

        if (isCanCancel && isCancelSameAccept)
        {
            cancelDayLimit = plan.CancelDayLimit;
            cancelLimit = plan.CancelLimit;
        }

        // チェックイン日の〇日前の〇時〇分
        var limit = checkInDate;
        if (cancelDayLimit is not null)
        {
            limit = limit.AddDays(-cancelDayLimit.Value);
        }

        if (cancelLimit is not null)
        {
            limit += cancelLimit.Value;
        }

        return now <= limit;
    }
}
