namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;

public class ReservationData
{
    public string? Code { get; set; }
    public long CheckInDate { get; set; }

    public int RestNumber { get; set; }

    public int RoomNumber { get; set; }

    public string? CheckInTime { get; set; }

    public string? CheckOutTime { get; set; }

    public bool IsSameMainUser { get; set; }
    public bool UseRoomUser { get; set; }

    public ReservationFacilityData? Facility { get; set; }

    public ReservationPlanData? Plan { get; set; }

    public ReservationRoomGroupData? RoomGroup { get; set; }

    public ReservationSiteData? Site { get; set; }

    public long UserId { get; set; }

    public ReservationUserData? Reserver { get; set; }

    public ReservationUserData? MainUser { get; set; }

    public string? PaymentType { get; set; }
    public int UsedPoint { get; set; }
    public int IncomePoint { get; set; }

    public string? Memo { get; set; }

    public List<ReservationPlanRoomGroupAppDateData>? ReservationPlanRoomGroupAppDates { get; set; }
    public List<ReservationQuestionData>? ReservationPlanQuestions { get; set; }
    public List<ReservationQuestionData>? ReservationOptionItemQuestions { get; set; }

    public ReservationData? Parent { get; set; }
    public List<ReservationData>? Children { get; set; }

    public List<ReservationUserData>? UserDatas => ReservationPlanRoomGroupAppDates?
        .Where(x => x.UserInfo is not null)
        .Select(a => a.UserInfo!)
        .ToList();

    public List<ReservationPersonAgeTypeData>? PersonAgeTypes => ReservationPlanRoomGroupAppDates?
        .SelectMany(
            a => a.ReservationRoomGroupAppDatePersonAgeTypes?.Where(
                x => x.PersonAgeType is not null
            ) ?? []
        )
        .Select(a => a.PersonAgeType!)
        .Distinct()
        .ToList();

    public List<ReservationRoomGroupAppDatePersonAgeTypeData>? ReservationPersonDatas =>
        ReservationPlanRoomGroupAppDates?
            .SelectMany(a => a.ReservationRoomGroupAppDatePersonAgeTypes ?? [])
            .ToList();

    public List<ReservationRoomGroupAppDatePersonAgeTypeData>? ReservationPriceDatas => ReservationPlanRoomGroupAppDates
        ?
        .SelectMany(a => a.ReservationRoomGroupAppDatePersonAgeTypes ?? [])
        .ToList();

    public List<ReservationRoomAppDateOptionItemData>? ReservationOptionItems => ReservationPlanRoomGroupAppDates?
        .SelectMany(a => a.ReservationRoomAppDateOptionItems ?? [])
        .ToList();

    /// <summary>
    /// 部屋料
    /// </summary>
    public int? TotalRoomGroupPrice => ReservationPlanRoomGroupAppDates?
        .SelectMany(a => a.ReservationRoomGroupAppDatePersonAgeTypes ?? [])
        .Sum(a => a.TotalPrice);

    /// <summary>
    /// 入湯税
    /// </summary>
    public int? TotalSpaTax => ReservationPlanRoomGroupAppDates?
        .SelectMany(a => a.ReservationRoomGroupAppDatePersonAgeTypes ?? [])
        .Sum(a => a.TotalSpaTax);

    /// <summary>
    /// オプションアイテム料
    /// </summary>
    public int? TotalOptionItemPrice => ReservationPlanRoomGroupAppDates?
        .SelectMany(a => a.ReservationRoomAppDateOptionItems ?? [])
        .Sum(a => a.TotalPrice);

    public int TotalPrice => TotalRoomGroupPrice ?? 0 + TotalSpaTax ?? 0 + TotalOptionItemPrice ?? 0;

    public int TotalDiscount => UsedPoint;

    public int AllTotalPrice => TotalPrice - TotalDiscount;

    public bool HasOptionItem => ReservationPlanRoomGroupAppDates?.Exists(
        a => a.HasOptionItem
    ) ?? false;
}

public class ReservationFileData
{
    /// <summary>
    /// ファイルID
    /// </summary>
    public long? Id { get; set; }

    /// <summary>
    /// コンテントタイプ
    /// </summary>
    public string? ContentType { get; set; }
}

public class ReservationFacilityData
{
    public long? Id { get; set; }
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }

    public bool? CanOnLinePayment { get; set; }
    public bool? IsOnSidePayment { get; set; }
    public bool? IsOnLinePayment { get; set; }

    public List<ReservationFileData>? Files { get; set; }
}

public class ReservationPlanData
{
    public bool? IsOnSidePayment { get; set; }
    public bool? IsOnLinePayment { get; set; }

    public long? Id { get; set; }
    public string? Name { get; set; }

    public PlanMeta? Meta { get; set; }

    public List<ReservationFileData>? Files { get; set; }
}

public class ReservationRoomGroupData
{
    public long? Id { get; set; }
    public string? Name { get; set; }
    public bool? IsEnabledSmoking { get; set; }
    public int? CapacityMax { get; set; }
    public int? CapacityMin { get; set; }
    public List<ReservationFileData>? Files { get; set; }
}

public class ReservationSiteData
{
    public long? Id { get; set; }
    public string? Name { get; set; }
}

public class ReservationPlanRoomGroupAppDateData
{
    public long AppDateId { get; set; }

    public int RestIndex { get; set; }
    public int RoomGroupIndex { get; set; }

    /// <summary>
    /// 部屋代表者情報
    /// </summary>
    public ReservationUserData? UserInfo { get; set; }

    public List<ReservationRoomGroupAppDatePersonAgeTypeData>? ReservationRoomGroupAppDatePersonAgeTypes { get; set; }

    public List<ReservationRoomAppDateOptionItemData>? ReservationRoomAppDateOptionItems { get; set; }

    public bool HasOptionItem => (ReservationRoomAppDateOptionItems?.Count ?? 0) > 0;
}

public class ReservationRoomGroupAppDatePersonAgeTypeData
{
    public long AppDateId { get; set; }

    public ReservationPersonAgeTypeData? PersonAgeType { get; set; }

    /// <summary>
    /// 日付順
    /// </summary>
    public int RestIndex { get; set; }

    /// <summary>
    /// 部屋順
    /// </summary>
    public int RoomGroupIndex { get; set; }

    /// <summary>
    /// 人数
    /// </summary>
    public int Persons { get; set; }

    /// <summary>
    /// 人数
    /// </summary>
    public int? MalePersons { get; set; }

    /// <summary>
    /// 人数
    /// </summary>
    public int? FemalePersons { get; set; }

    /// <summary>
    /// 人数
    /// </summary>
    public int? NonePeersons { get; set; }

    /// <summary>
    /// 単価
    /// </summary>
    public int Price { get; set; }

    /// <summary>
    /// 入湯税単価
    /// </summary>
    public int SpaTax { get; set; }

    /// <summary>
    /// 部屋料金小計
    /// </summary>
    public int TotalPrice { get; set; }

    /// <summary>
    /// 入湯税小計
    /// </summary>
    public int TotalSpaTax { get; set; }
}

public class ReservationRoomAppDateOptionItemData
{
    public int Price { get; set; }

    public long AppDateId { get; set; }

    /// <summary>
    /// 日付順
    /// </summary>
    public int RestIndex { get; set; }

    /// <summary>
    /// 部屋順
    /// </summary>
    public int RoomGroupIndex { get; set; }

    public ReservationOptionItemData? OptionItem { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    public int Number { get; set; }

    /// <summary>
    /// 小計
    /// </summary>
    public int TotalPrice { get; set; }

    /// <summary>
    /// 小計が数量単価と一致しているか？
    /// 登録時にバリデーションするが、理論上起こりえるので本プロパティを設置する
    /// </summary>
    public bool IsInvalidTotalPrice => TotalPrice != (OptionItem?.Price ?? 0) * Number;
}

public class ReservationOptionItemData
{
    public long? Id { get; set; }

    /// <summary>
    /// オプション名
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 単価
    /// </summary>
    public int? Price { get; set; }
}

public class ReservationPersonAgeTypeData
{
    public long? Id { get; set; }

    public string? Name { get; set; }
    public bool? IsMain { get; set; }

    /// <summary>
    /// 年齢上限
    /// </summary>
    public int? AgeMax { get; set; }

    /// <summary>
    /// 年齢下限
    /// </summary>
    public int? AgeMin { get; set; }

    // public override bool Equals(
    //     object? obj
    // )
    // {
    //     if (obj is not ReservationPersonAgeTypeData data)
    //     {
    //         return false;
    //     }
    //
    //     return data.Id == Id;
    // }
}

public class ReservationQuestionData
{
    public ReservationQuestionData2? Question { get; set; }

    /// <summary>
    /// 回答データ
    /// </summary>
    public string? AnswerData { get; set; }
}

public class ReservationQuestionData2
{
    /// <summary>
    /// 質問事項ID
    /// </summary>
    public long? Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    /// <summary>
    /// 種別
    /// </summary>
    public string? QuestionType { get; set; }

    public string? FormData { get; set; }
}

public class ReservationUserData
{
    public long AppDateId { get; set; }

    public int RestIndex { get; set; }
    public int RoomGroupIndex { get; set; }

    /// <summary>
    /// 名前
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// かな
    /// </summary>
    public string? Kana { get; set; }

    /// <summary>
    /// メールアドレス
    /// </summary>
    public string? EMail { get; set; }

    /// <summary>
    /// 英語表記名
    /// </summary>
    public string? NameE { get; set; }

    /// <summary>
    /// 性別
    /// </summary>
    public string? Gender { get; set; }

    /// <summary>
    /// 郵便番号
    /// </summary>
    public string? PostCode { get; set; }

    public string? CountryCode { get; set; }

    public string? Prefecture { get; set; }

    /// <summary>
    /// 住所
    /// </summary>
    public string? Address1 { get; set; }

    public string? Address2 { get; set; }
    public string? Address3 { get; set; }

    /// <summary>
    /// 電話番号
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 誕生日
    /// </summary>
    public long? Birthday { get; set; }
}
