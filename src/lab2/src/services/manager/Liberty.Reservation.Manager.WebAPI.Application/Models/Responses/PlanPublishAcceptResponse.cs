namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record PlanPublishAcceptResponse
{
    public long Id { get; init; }
    public bool UseBookingReception { get; init; }
    public long? BookingReceptionStart { get; init; }
    public long? BookingReceptionEnd { get; init; }
    public bool UseDisplayDate { get; init; }
    public long? DisplayDateStart { get; init; }
    public long? DisplayDateEnd { get; init; }
    public bool UseAcceptDate { get; init; }
    public long? AcceptDateStart { get; init; }
    public long? AcceptDateEnd { get; init; }
    public int? AcceptDays { get; init; }
    public int? AcceptMonths { get; init; }
    public PlanAcceptEndLimitTypes? AcceptEndLimitType { get; init; }
    public int? ReceptionDayLimit { get; init; }
    public TimeSpan? ReceptionLimit { get; init; }
    public bool? UseReceptionStartDay { get; set; }
    public int? ReceptionStartDayLimit { get; set; }
    public TimeSpan? ReceptionStartLimit { get; set; }
    public IEnumerable<SiteOfPlanPublishAcceptResponse>? Sites { get; init; }
}

public record SiteOfPlanPublishAcceptResponse(
    long Id,
    string? Name,
    bool IsEnabled
);
