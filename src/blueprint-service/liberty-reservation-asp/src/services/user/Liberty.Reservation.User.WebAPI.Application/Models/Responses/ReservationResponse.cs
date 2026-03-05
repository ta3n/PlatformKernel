using Liberty.Reservation.Application.Constants;

namespace Liberty.Reservation.User.WebAPI.Application.Models.Responses;

public record ReservationResponse
{
    public long Id { get; init; }
    public string? Code { get; init; }
    public string? MediaCode { get; init; }
    public ReservationStatus State { get; init; }
    public string? Facilityname { get; init; }
    public bool CanOnLinePayment { get; init; }
    public string? RoomGroupName { get; init; }
    public string? PlanName { get; init; }
    public string? SiteName { get; init; }
    public IEnumerable<MealTypeOfReservationResponse>? MealTypes { get; init; }
    public DateTime CheckInDate { get; init; }
    public int RestNumber { get; init; }
    public TimeSpan? CheckOutTime { get; init; }
    public DateTime ReservationDateTime { get; init; }
    public bool IsEnabledSmoking { get; init; }
    public int LengthOfStay { get; init; }
    public int NumberOfRooms { get; init; }
    public decimal? TotalPrice { get; init; }
    public string? PaymentType { get; init; }
    public string? Memo { get; init; }
    public TimeSpan? CheckInTime { get; init; }
    public int? ReceptionDayLimit { get; init; }
    public TimeSpan? ReceptionLimit { get; init; }
    public bool IsCancelSameAccept { get; init; }
    public int? CancelDayLimit { get; init; }
    public TimeSpan? CancelLimit { get; init; }
    public int TimeZoneOffset { get; init; }
    public PlanTypes PlanType { get; set; }
    public bool IsNoShow { get; init; }
    public DateTime? NoShowDateTime { get; init; }
    public string? NoShowReason { get; init; }
    public bool? DayUse { get; init; }
    public DateTime? UpdatedAt { get; set; }
    public int UpdateCount { get; init; }
    public bool IsDiffAmount { get; init; }
    public string? RootCode { get; init; }
    public bool IsChange => CanModify(false);

    public bool IsCancel => CanModify();

    public bool CanModify(
        bool isCanCancel = true
    )
    {
        if (State is not (ReservationStatus.Confirmed or ReservationStatus.Reserved))
        {
            return false;
        }

        var now = DateTime.UtcNow.AddHours(TimeZoneOffset);
        var checkInDate = CheckInTime.HasValue ? CheckInDate.Add(CheckInTime.Value) : CheckInDate;

        var cancelDayLimit = ReceptionDayLimit;
        var cancelLimit = ReceptionLimit;
        var isCancelSameAccept = !IsCancelSameAccept;

        if (isCanCancel && isCancelSameAccept)
        {
            cancelDayLimit = CancelDayLimit;
            cancelLimit = CancelLimit;
        }

        var limit = checkInDate;
        if (cancelDayLimit is not null)
        {
            limit = limit.AddDays(-cancelDayLimit.Value);
        }

        if (cancelLimit is not null)
        {
            limit += cancelLimit.Value;
        }

        if (now > limit)
        {
            return false;
        }

        return (checkInDate - now).TotalSeconds > 0;
    }
}

public record MealTypeOfReservationResponse(
    long Id,
    MealTypeEatTypes MealTypeEatType,
    string? Name
);
