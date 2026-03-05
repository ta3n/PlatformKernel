namespace Liberty.Reservation.Application.Models;

public record BookingMetaStandardPriceModel(
    long PlanId,
    long RoomGroupId,
    long AppDateTypeId,
    int? PersonMin,
    int? PersonMax,
    int? Price
);
