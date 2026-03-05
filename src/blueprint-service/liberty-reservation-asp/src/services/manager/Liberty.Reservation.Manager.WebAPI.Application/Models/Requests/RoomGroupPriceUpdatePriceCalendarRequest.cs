namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record RoomGroupPriceUpdatePriceCalendarRequest(
    List<RomTypeUpdatePriceDataCalendarRequest>? PriceDatas
);

public record RomTypeUpdatePriceDataCalendarRequest(
    long DateCalendar,
    int? PersonMin,
    int? PersonMax,
    int? Price,
    bool UseAutoDiscount
);
