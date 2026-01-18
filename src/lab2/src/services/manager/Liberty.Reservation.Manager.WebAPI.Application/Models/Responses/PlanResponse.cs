namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record PlanResponse
{
    /// <summary>
    /// プランID
    /// </summary>
    public long Id { get; init; }

    /// <summary>
    /// プラン名
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// 掲載期間設定を利用するか
    /// </summary>
    public bool UseDisplayDate { get; init; }

    /// <summary>
    /// 掲載期間開始
    /// </summary>
    public long? DisplayDateStart { get; init; }

    /// <summary>
    /// 掲載期間終了
    /// </summary>
    public long? DisplayDateEnd { get; init; }

    public long? BookingReceptionEnd { get; init; }

    public long? BookingReceptionStart { get; init; }

    public bool UseBookingReception { get; init; }

    /// <summary>
    /// 受付期間を利用するか？
    /// </summary>
    public bool UseAcceptDate { get; init; }

    /// <summary>
    /// 受付期間開始
    /// </summary>
    public long? AcceptDateStart { get; init; }

    /// <summary>
    /// 受付期間終了
    /// </summary>
    public long? AcceptDateEnd { get; init; }

    public bool IsEnabled { get; init; }

    /// <summary>
    /// 現地決済か？
    /// </summary>
    public bool IsOnSidePayment { get; init; }

    /// <summary>
    /// オンライン決済か？
    /// </summary>
    public bool IsOnLinePayment { get; init; }

    public bool HasWarning { get; init; }

    /// <summary>
    /// タグ
    /// </summary>
    public string[]? Tags { get; init; }

    public List<CategoryOfPlanResponse> Categories { get; init; } = [];

    public List<FileOfPlanResponse> Files { get; init; } = [];

    public List<RoomGroupOfPlanResponse> PlanRoomTypes { get; init; } = [];

    public List<long> Warnings { get; init; } = [];

    public long? DisplayOrder { get; init; }

    public PlanTypes? PlanType { get; init; }

    public TimeSpan? CheckInStart { get; set; }

    public TimeSpan? CheckInEnd { get; set; }

    public TimeSpan? CheckOut { get; set; }

    public List<string> MealTypes { get; set; } = [];

    public List<string> Sites { get; set; } = [];
}

public record CategoryOfPlanResponse(
    long Id,
    string? Name,
    bool IsEnabled
);

public record FileOfPlanResponse(
    long Id,
    string? Code,
    bool IsEnabled,
    int Index,
    string? Description
);

public record RoomGroupOfPlanResponse(
    bool IsUsed,
    long RoomTypeId,
    string? Name,
    bool IsEnabled,
    bool IsEnabledOfRelation,
    int BaseNumber,
    int? CapacityMin,
    int? CapacityMax,
    decimal? Size,
    RoomGroupSizeUnitTypes RoomGroupSizeUnitType,
    bool IsEnabledSmoking
);
