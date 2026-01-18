namespace Liberty.Reservation.Application.Models.Responses;

public record BookingCalendarPriceResponse(
    long PlanId,
    long RoomGroupId,
    PriceCalendarOfRoomResponse PriceCalendar
);
