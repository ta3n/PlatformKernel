namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record RoomGroupDetailPublicationSettingResponse
{
    public long Id { get; init; }
    public IEnumerable<SiteOfRoomGroupDetailPublicationSettingResponse>? Sites { get; set; }
    public bool? UseDisplayDate { get; set; }
    public long? DisplayDateStart { get; set; }
    public long? DisplayDateEnd { get; set; }
    public bool? UseAcceptDate { get; set; }
    public long? AcceptDateStart { get; set; }
    public long? AcceptDateEnd { get; set; }
    public int? AcceptDays { get; set; }
    public int? AcceptMonths { get; set; }
    public PlanAcceptEndLimitTypes? AcceptEndLimitType { get; set; }
    public int? ReceptionDayLimit { get; set; }
    public TimeSpan? ReceptionLimit { get; set; }
}

public record SiteOfRoomGroupDetailPublicationSettingResponse(
    long Id,
    string? Name
);
