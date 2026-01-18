namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record PlanRoomSiteMinimumPriceResponse
{
    public long PlanId { get; set; }
    public long RoomId { get; set; }
    public long SiteId { get; set; }
    public bool IsEnabledMinimumPrice { get; set; }
    public int? MinimumPrice { get; set; }
}
