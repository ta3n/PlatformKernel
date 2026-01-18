namespace Liberty.Reservation.Application.Models;

public record BookingMetaPlanAppDateModel(
    long PlanId,
    long RoomGroupId,
    long SiteId,
    long AppDateId,
    bool UseAutoDiscount,
    float? PointRate
);
