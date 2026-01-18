using Liberty.ApplicationShared.Utils;
using Liberty.Entity;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

/// <summary>
/// 予約情報
/// </summary>
public class Reservation : EntityData
{
    public string? Serial { get; set; }

    public Facility? Facility { get; set; }
    public Plan? Plan { get; set; }
    public RoomGroup? RoomGroup { get; set; }
    public Site? Site { get; set; }

    public long UserId { get; set; }

    /// <summary>
    /// 予約者
    /// </summary>
    public User? User { get; set; }

    /// <summary>
    /// 登録済みの予約者がいるか？
    /// </summary>
    public bool HasApplicationUser => User is not null;

    /// <summary>
    /// 予約者情報
    /// </summary>
    public UserInfo? Reserver { get; set; }

    /// <summary>
    /// 宿泊代表者
    /// </summary>
    public UserInfo? MainUser { get; set; }

    public long CheckInDate { get; set; }

    public long CheckOutDate
    {
        get
        {
            var checkOutDate = AppDate.GetDateTime(CheckInDate).AddDays(RestNumber);
            //
            return AppDate.GetId(checkOutDate);
        }
    }

    public int RestNumber { get; set; }

    public int RoomNumber { get; set; }

    public TimeSpan? CheckInTime { get; set; }

    public TimeSpan? CheckOutTime { get; set; }

    public PaymentTypes PaymentType { get; set; }

    public int UsedPoint { get; set; }

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
                .Order?.OrderGMOPaymentResultRequests?.FirstOrDefault();

            return orderGmoPaymentResultRequest?.GmoPaymentResultRequest;
        }
    }

    public bool IsOnSidePayment => PaymentType == PaymentTypes.OnSidePayment;

    public bool IsOnlinePayment => PaymentType == PaymentTypes.OnLinePayment;

    /// <summary>
    /// オンライン決済可能な予約か？
    /// </summary>
    public bool CanOnlinePayment
    {
        get
        {
            if (Facility is null || Plan is null)
            {
                return false;
            }

            // 施設がオンライン決済可能か？
            if (!Facility.CanOnLinePayment || !Facility.IsOnLinePayment)
            {
                return false;
            }

            // プランがオンライン決済可能か？
            if (!Plan.IsOnLinePayment)
            {
                return false;
            }

            // 過去に取り消し条件があればオンライン決済可能でなくす
            // 取り消し処理があるか？
            if (HasGmoPaymentVoid)
            {
                return false;
            }

            // 返品処理があるか？
            if (HasGmoPaymentReturn)
            {
                return false;
            }

            // 月跨り返品があるか？
            if (HasGmoPaymentReturnX)
            {
                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// GMOペイメントのリクエストリスト
    /// </summary>
    public ICollection<OrderGmoPaymentResultRequest>? OrderGmoPaymentResultRequests => OrderReservations?
        .Select(a => a.Order)
        .SelectMany(a => a?.OrderGMOPaymentResultRequests ?? [])
        .ToList();

    /// <summary>GMOペイメントに取消があるか？</summary>
    public bool HasGmoPaymentVoid =>
        OrderGmoPaymentResultRequests?.Any(
            a => a.GmoPaymentResultRequest?.IsVoid ?? false
        ) ?? false;

    /// <summary>GMOペイメントに返品があるか？</summary>
    public bool HasGmoPaymentReturn =>
        OrderGmoPaymentResultRequests?.Any(
            a => a.GmoPaymentResultRequest?.IsReturn ?? false
        ) ?? false;

    /// <summary>GMOペイメントに月跨り返品があるか？</summary>
    public bool HasGmoPaymentReturnX =>
        OrderGmoPaymentResultRequests?.Any(
            a => a.GmoPaymentResultRequest?.IsReturnX ?? false
        ) ?? false;

    /// <summary>
    /// GMOペイメントの支払い処理がすべて完了しているか？
    /// </summary>
    public bool IsGmoPaymentCompleteAll()
    {
        // 取り消し処理があるか？
        if (HasGmoPaymentVoid)
        {
            return false;
        }

        // 返品処理があるか？
        if (HasGmoPaymentReturn)
        {
            return false;
        }

        // 月跨り返品があるか？
        if (HasGmoPaymentReturnX)
        {
            return false;
        }

        var amount = Amount;
        var tax = SpaTax;

        // 決済完了している支払いで抽出
        var gmoPaymentResultRequests = OrderGmoPaymentResultRequests?
            .Where(
                a => a.GmoPaymentResultRequest is
                {
                    IsCapture: true,
                    IsCompleted: true
                }
            );

        var sAmount = gmoPaymentResultRequests?.Sum(
            a => long.Parse(a.GmoPaymentResultRequest?.Amount ?? "0")
        ) ?? 0;
        var sTax = gmoPaymentResultRequests?.Sum(
            a => long.Parse(a.GmoPaymentResultRequest?.Tax ?? "0")
        ) ?? 0;

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
    public int? Amount => ReservationPlanRoomGroupAppDates?.Sum(
        a => a.Amount
    );

    /// <summary>
    /// 入湯税
    /// </summary>
    public int? SpaTax => ReservationPlanRoomGroupAppDates?.Sum(
        a => a.SpaTax
    );

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
    public ReservationStatus ReservationState { get; set; } = ReservationStatus.Unknown;

    public DateTime? ModifiedDateTime { get; set; }
    public DateTime? CancelledDateTime { get; set; }
    public int? CancellationPrice { get; set; }

    public DateTime? NoShowDateTime { get; set; }
    public string? NoShowReason { get; set; }

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
    public bool IsUnConfirmed => ReservationState == ReservationStatus.UnConfirmed;

    /// <summary>
    /// 予約成立状態か？
    /// </summary>
    public bool IsReserved => ReservationState == ReservationStatus.Confirmed;

    /// <summary>
    /// 予約キャンセル状態か？
    /// </summary>
    public bool IsCancelled => ReservationState == ReservationStatus.Canceled;

    /// <summary>
    /// 予約変更済みの状態か？
    /// </summary>
    public bool IsModified => ReservationState == ReservationStatus.Modified;

    /// <summary>
    /// NoShow処理された状態か？？
    /// </summary>
    public bool IsNoShowed => ReservationState == ReservationStatus.NoShow;

    /// <summary>
    /// 編集中か？
    /// </summary>
    public bool IsModifying => ReservationState == ReservationStatus.Modifing;

    /// <summary>
    /// オンライン予約未決済中か？
    /// </summary>
    public bool IsPaymentNotApproved => ReservationState == ReservationStatus.PaymentNotApproved;

    /// <summary>
    /// 情報JSON
    /// </summary>
    public string JsonData { get; set; } = "{}";

    public ReservationData? ReservationData
    {
        get => JsonConvert.DeserializeObject<ReservationData>(JsonData);
        set => JsonData = JsonConvert.SerializeObject(value);
    }

    public bool IsSameMainUser { get; set; }
    public bool UseRoomUser { get; set; }

    /// <summary>キャンセル料対象料金取得</summary>
    /// <returns></returns>
    public int CancellationTargetPrice
    {
        get
        {
            // 計算 宿泊料金、オプション料金が対象
            var totalPrice = 0;
            totalPrice += ReservationData?.TotalRoomGroupPrice ?? 0;
            totalPrice += ReservationData?.TotalOptionItemPrice ?? 0;

            return totalPrice;
        }
    }

    public bool CanCancel(
        DateTime now
    )
    {
        // 以下の条件の場合変更・キャンセル不可
        if (IsCancelled)
        {
            return false;
        }

        if (IsModified)
        {
            return false;
        }

        if (IsNoShowed)
        {
            return false;
        }

        if (IsModifying)
        {
            return false;
        }

        //
        var checkInDate = AppDate.GetDateTime(CheckInDate);

        var plan = Plan;
        if (plan is null)
        {
            return false;
        }

        var isCancelSameAccept = plan.IsCancelSameAccept;
        var cancelDayLimit = isCancelSameAccept ? plan.ReceptionDayLimit : plan.CancelDayLimit;
        var cancelLimit = isCancelSameAccept ? plan.ReceptionLimit : plan.CancelLimit;

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

        // 指定時刻との差があればOK
        return (checkInDate - now).TotalSeconds > 0;
    }

    public bool CanModify(
        DateTime now
    )
    {
        // キャンセル可能期間と同じとする
        return CanCancel(now);
    }

    /// <summary>
    /// 現在の内容で予約内容ほ保存します
    /// </summary>
    public Task SaveReservationDataAsync()
    {
        var reservationPlanRoomGroupAppDates = ReservationPlanRoomGroupAppDates?.Select(
                a =>
                {
                    return new ReservationPlanRoomGroupAppDateData
                    {
                        AppDateId = a.AppDateId,
                        RestIndex = a.RestIndex,
                        RoomGroupIndex = a.RoomGroupIndex,
                        UserInfo = new ReservationUserData
                        {
                            AppDateId = a.AppDateId,
                            RestIndex = a.RestIndex,
                            RoomGroupIndex = a.RoomGroupIndex,
                            Name = a.UserInfo?.Name,
                            Kana = a.UserInfo?.Kana,
                            EMail = a.UserInfo?.EMail,
                            NameE = a.UserInfo?.NameE,
                            Gender = a.UserInfo?.Gender.ToString(),
                            Birthday = a.UserInfo?.BirthDay,
                            CountryCode = a.UserInfo?.Country?.Code,
                            PostCode = a.UserInfo?.PostCode,
                            Address1 = a.UserInfo?.Address1,
                            Address2 = a.UserInfo?.Address2,
                            Address3 = a.UserInfo?.Address3,
                            Phone = a.UserInfo?.Phone
                        },
                        ReservationRoomGroupAppDatePersonAgeTypes = a.ReservationRoomGroupAppDatePersonAgeTypes
                            ?
                            .Select(
                                b => new ReservationRoomGroupAppDatePersonAgeTypeData
                                {
                                    AppDateId = b.AppDateId,
                                    RestIndex = b.RestIndex,
                                    RoomGroupIndex = b.RoomGroupIndex,
                                    Persons = b.Number,
                                    MalePersons = b.MaleNumber,
                                    FemalePersons = b.FemaleNumber,
                                    NonePeersons = b.GenderNoneNumber,
                                    Price = b.UnitPrice,
                                    SpaTax = b.SpaTax,
                                    TotalPrice = b.TotalRoomGroupPrice,
                                    TotalSpaTax = b.TotalSpaTax,
                                    PersonAgeType = new ReservationPersonAgeTypeData
                                    {
                                        Id = b.PersonAgeType?.Id,
                                        Name = b.PersonAgeType?.Name,
                                        IsMain = b.PersonAgeType?.IsMain,
                                        AgeMax = b.PersonAgeType?.AgeMax,
                                        AgeMin = b.PersonAgeType?.AgeMin
                                    }
                                }
                            )
                            .ToList(),
                        ReservationRoomAppDateOptionItems = a.ReservationRoomGroupAppDateOptionItems
                            ?
                            .Select(
                                b => new ReservationRoomAppDateOptionItemData
                                {
                                    AppDateId = b.AppDateId,
                                    RestIndex = b.RestIndex,
                                    RoomGroupIndex = b.RoomGroupIndex,
                                    OptionItem = new ReservationOptionItemData
                                    {
                                        Id = b.OptionItem?.Id,
                                        Name = b.OptionItem?.Name,
                                        Price = b.OptionItem?.Price
                                    },
                                    Number = b.Number,
                                    Price = b.Price,
                                    TotalPrice = b.TotalPrice
                                }
                            )
                            .ToList()
                    };
                }
            )
            .ToArray();

        var reservationPlanQuestions = ReservationQuestions?
            .Where(a => a.ReservationQuestionType == ReservationQuestionTypes.Plan)
            .Select(
                a => new ReservationQuestionData
                {
                    Question = new ReservationQuestionData2
                    {
                        Id = a.Question?.Id,
                        Name = a.Question?.Name,
                        Description = a.Question?.Description,
                        QuestionType = a.ReservationQuestionType.ToString()
                    },
                    AnswerData = a.AnswerData
                }
            )
            .ToArray();

        var reservationOptionItemQuestions = ReservationQuestions?
            .Where(a => a.ReservationQuestionType == ReservationQuestionTypes.OptionItem)
            .Select(
                a => new ReservationQuestionData
                {
                    Question = new ReservationQuestionData2
                    {
                        Id = a.Question?.Id,
                        Name = a.Question?.Name,
                        Description = a.Question?.Description,
                        QuestionType = a.ReservationQuestionType.ToString()
                    },
                    AnswerData = a.AnswerData
                }
            )
            .ToArray();

        var data = new ReservationData
        {
            Code = Code,
            CheckInDate = CheckInDate,
            RestNumber = RestNumber,
            RoomNumber = RoomNumber,
            CheckInTime = ConvertUtil.ToString(CheckInTime, @"hh\:mm"),
            CheckOutTime = ConvertUtil.ToString(CheckOutTime, @"hh\:mm"),
            PaymentType = PaymentType.ToString(),
            UsedPoint = UsedPoint,
            Memo = Memo,
            IsSameMainUser = IsSameMainUser,
            UseRoomUser = UseRoomUser,
            IncomePoint = IncomePoint,
            Parent = Parent is null
                ? null
                : new ReservationData { Code = Parent.Code },
            Facility = new ReservationFacilityData
            {
                Id = Facility?.Id,
                Name = Facility?.Name,
                Address = Facility?.Address,
                Phone = Facility?.Phone,
                CanOnLinePayment = Facility?.CanOnLinePayment,
                IsOnSidePayment = Facility?.IsOnSidePayment,
                IsOnLinePayment = Facility?.IsOnLinePayment,
                Files = Facility?.FacilityFiles
                    ?
                    .Select(
                        a => new ReservationFileData
                        {
                            Id = a.File?.Id,
                            ContentType = a.File?.ContentType
                        }
                    )
                    .ToList()
            },
            Plan = new ReservationPlanData
            {
                Id = Plan?.Id,
                Name = Plan?.Name,
                Meta = Plan?.Meta,
                IsOnSidePayment = Plan?.IsOnSidePayment,
                IsOnLinePayment = Plan?.IsOnLinePayment,
                Files = Plan?.FilePlans
                    ?
                    .Select(
                        a => new ReservationFileData
                        {
                            Id = a.File?.Id,
                            ContentType = a.File?.ContentType
                        }
                    )
                    .ToList()
            },
            RoomGroup = new ReservationRoomGroupData
            {
                Id = RoomGroup?.Id,
                Name = RoomGroup?.Name,
                IsEnabledSmoking = RoomGroup?.IsEnabledSmoking,
                CapacityMax = RoomGroup?.CapacityMax,
                CapacityMin = RoomGroup?.CapacityMin,
                Files = RoomGroup?.FileRoomGroups
                    ?
                    .Select(
                        a => new ReservationFileData
                        {
                            Id = a.File?.Id,
                            ContentType = a.File?.ContentType
                        }
                    )
                    .ToList()
            },
            Site = new ReservationSiteData
            {
                Id = Site?.Id,
                Name = Site?.Name
            },
            UserId = UserId,
            Reserver = new ReservationUserData
            {
                Name = Reserver?.Name,
                Kana = Reserver?.Kana,
                EMail = Reserver?.EMail,
                NameE = Reserver?.NameE,
                Gender = Reserver?.Gender.ToString(),
                Birthday = Reserver?.BirthDay,
                CountryCode = Reserver?.Country?.Code,
                PostCode = Reserver?.PostCode,
                Address1 = Reserver?.Address1,
                Address2 = Reserver?.Address2,
                Address3 = Reserver?.Address3,
                Phone = Reserver?.Phone
            },
            MainUser = new ReservationUserData
            {
                Name = MainUser?.Name,
                Kana = MainUser?.Kana,
                EMail = MainUser?.EMail,
                NameE = MainUser?.NameE,
                Gender = MainUser?.Gender.ToString(),
                Birthday = MainUser?.BirthDay,
                CountryCode = MainUser?.Country?.Code,
                PostCode = MainUser?.PostCode,
                Address1 = MainUser?.Address1,
                Address2 = MainUser?.Address2,
                Address3 = MainUser?.Address3,
                Phone = MainUser?.Phone
            },
            ReservationPlanRoomGroupAppDates = reservationPlanRoomGroupAppDates?.ToList() ?? [],
            ReservationPlanQuestions = reservationPlanQuestions?.ToList() ?? [],
            ReservationOptionItemQuestions = reservationOptionItemQuestions?.ToList() ?? []
        };

        ReservationData = data;

        return Task.CompletedTask;
    }

    public Reservation()
    {
    }

    public Reservation(
        Facility facility,
        Plan plan,
        Site site
    )
    {
        Facility = facility;
        Site = site;
        Plan = plan;
    }

    public static string CreatePassString(
        string? format,
        long? reservationId,
        string? reservationCode
    )
    {
        if (string.IsNullOrEmpty(format))
        {
            return string.Empty;
        }

        //
        var v1 = EncryptUtil.MD5(ConvertUtil.ToString(reservationId));
        var v2 = EncryptUtil.MD5(reservationCode);

        return ConvertUtil.Format(format, v1, v2);
    }

    public async Task ReserveAsync(
        DateTime reservationDateTime
    )
    {
        // 予約完了状態
        if (ReservationState != ReservationStatus.Confirmed)
        {
            // 予約完了
            if (HasApplicationUser)
            {
                // ログイン状態で現地決済は「認証済み」とする
                if (IsOnSidePayment)
                {
                    Confirmed(reservationDateTime);
                }
                else if (IsOnlinePayment)
                    // 「オンライン決済」を選択し予約直後は「オンライン決済承認」とする
                {
                    ReservationState = ReservationStatus.PaymentNotApproved;
                }
            }
            else if (!HasApplicationUser)
            {
                // 予約要承認
                UnConfirmed();
            }
        }

        // 予約時刻設定
        ReservationDateTime = reservationDateTime;

        // 予約時の内容をJSONに保存する
        await SaveReservationDataAsync();
    }

    public void PayOnlinePayment(
        DateTime reservationDateTime
    )
    {
        // JSONも変更
        if (ReservationData is not null)
        {
            var jsonData = ReservationData;
            jsonData.PaymentType = PaymentType.ToString();
            ReservationData = jsonData;
        }

        Confirmed(reservationDateTime);
    }

    public void Confirmed(
        DateTime confirmedDateTime
    )
    {
        ReservationState = ReservationStatus.Confirmed;
        ConfirmedDateTime = confirmedDateTime;
    }

    public void UnConfirmed()
    {
        ReservationState = ReservationStatus.UnConfirmed;
    }

    public void Cancel(
        DateTime cancelledDateTime
    )
    {
        // キャンセル料を計算し保持
        var cancellationPrice = Plan?.Cancellation?.GetCancellationPrice(
            CancellationTargetPrice,
            AppDate.GetDateTime(CheckInDate),
            cancelledDateTime
        );

        CancellationPrice = cancellationPrice;
        ReservationState = ReservationStatus.Canceled;
        CancelledDateTime = cancelledDateTime;
    }

    public void NoShow(
        string reason,
        DateTime noShowDateTime
    )
    {
        ReservationState = ReservationStatus.NoShow;
        NoShowReason = reason;
        NoShowDateTime = noShowDateTime;
    }

    public void Modify(
        Reservation newReservation,
        DateTime modifiedDateTime
    )
    {
        // 現在の予約を変更状態にする
        ReservationState = ReservationStatus.Modified;
        ModifiedDateTime = modifiedDateTime;

        // 親子関係を構築する
        newReservation.Parent = this;
    }

    public TimeSpan Expire(
        DateTime date,
        int minutes
    )
    {
        var expire = ReservationDateTime.AddMinutes(minutes) - date;

        return expire;
    }
}
