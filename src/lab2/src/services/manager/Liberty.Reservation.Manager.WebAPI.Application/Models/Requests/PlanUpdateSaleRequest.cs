namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record PlanUpdateSaleRequest(
    TimeSpan? CheckInStart,
    TimeSpan? CheckInEnd,
    TimeSpan? CheckOut,
    int? TimeIntervalMinutes,
    int? RoomNumberDaySaleLimit,
    int? GroupNumberDaySaleLimit,
    PlanDaySaleLimitTypes? PlanDaySaleLimitType,
    bool? UseAcceptPersonNumber,
    int? AcceptPersonNumberMin,
    int? AcceptPersonNumberMax,
    bool? UseDaySaleLimit,
    int? NumberOfStayLimitMin,
    int? NumberOfStayLimitMax,
    float? PointRate,
    int? PointExpire
);
