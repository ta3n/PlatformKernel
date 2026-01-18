namespace Liberty.Reservation.Application.Models;

public record BookingMetaRoomAppDateModel(
    long RoomGroupId,
    long AppDateId,
    bool IsNotSelled,
    int? RoomCapacityMax,
    int? SellNumber,
    int? RemainNumber,
    int ReservedNumber
);
