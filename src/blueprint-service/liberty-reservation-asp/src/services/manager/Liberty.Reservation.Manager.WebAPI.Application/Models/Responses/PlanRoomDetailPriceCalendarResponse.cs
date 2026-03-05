namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record PlanRoomDetailPriceCalendarResponse(
    long PlanId,
    long RomTypeId,
    long SiteId,
    List<RoomTypePriceDataCalendarResponse>? PriceDatas
);

public record RoomTypePriceDataCalendarResponse
{
    public long DateCalendar { get; init; }
    public int? PersonMin { get; init; }
    public int? PersonMax { get; init; }
    public int? Price { get; init; }
    public bool UseAutoDiscount { get; set; }
}

public record RoomTypePriceCalendarDataResponse
{
    public long DateCalendar { get; init; }
    public bool UseAutoDiscount { get; init; }
    public float? PointRate { get; init; }
}
