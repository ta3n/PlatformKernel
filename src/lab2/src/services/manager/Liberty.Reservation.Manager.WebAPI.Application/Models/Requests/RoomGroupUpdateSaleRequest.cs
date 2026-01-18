namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record RoomGroupUpdateSaleRequest(
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
) : PlanUpdateSaleRequest(
    CheckInStart,
    CheckInEnd,
    CheckOut,
    TimeIntervalMinutes,
    RoomNumberDaySaleLimit,
    GroupNumberDaySaleLimit,
    PlanDaySaleLimitType,
    UseAcceptPersonNumber,
    AcceptPersonNumberMin,
    AcceptPersonNumberMax,
    UseDaySaleLimit,
    NumberOfStayLimitMin,
    NumberOfStayLimitMax,
    PointRate,
    PointExpire
);
