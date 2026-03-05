namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record RoomGroupUpdatePublicationSettingRequest(
    long[]? SiteIds,
    bool? UseDisplayDate,
    long? DisplayDateStart,
    long? DisplayDateEnd,
    bool? UseAcceptDate,
    long? AcceptDateStart,
    long? AcceptDateEnd,
    int? AcceptDays,
    int? AcceptMonths,
    PlanAcceptEndLimitTypes? AcceptEndLimitType,
    int? ReceptionDayLimit,
    TimeSpan? ReceptionLimit
)
{
    public long? Id { get; set; }
}
