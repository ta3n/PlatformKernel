namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record PlanSaleResponse
{
    public long Id { get; init; }
    public TimeSpan? CheckInStart { get; init; }
    public TimeSpan? CheckInEnd { get; init; }
    public string? CheckOut { get; init; }
    public int TimeIntervalMinutes { get; init; }
    public int? RoomNumberDaySaleLimit { get; init; }
    public int? GroupNumberDaySaleLimit { get; init; }
    public PlanDaySaleLimitTypes? PlanDaySaleLimitType { get; init; }
    public bool UseDaySaleLimit { get; init; }
    public bool UseAcceptPersonNumber { get; init; }
    public int? AcceptPersonNumberMin { get; init; }
    public int? AcceptPersonNumberMax { get; init; }
    public int? NumberOfStayLimitMin { get; init; }
    public int? NumberOfStayLimitMax { get; init; }
    public float? PointRate { get; init; }
    public int? PointExpire { get; init; }
    public int? MaxRoomNumberDaySaleLimit { get; init; }
    public bool DayUse { get; init; }
}
