namespace Liberty.Reservation.Application.Models;

public record BookingMetaRangePersonModel(
    long PlanId,
    long RoomGroupId,
    int? PersonMin,
    int? PersonMax
);
