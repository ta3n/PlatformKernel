namespace Liberty.Reservation.Application.Models;

public record RoomInventoryModel(
    long FacilityId,
    long RoomId,
    long AppDateId,
    int MaxQuantity,
    int NumberOfReservedForUser,
    int NumberOfReservedForGuest,
    int NumberOfReservedAnotherPlan
)
{
    public int ReservedNumber { get; } = NumberOfReservedForUser + NumberOfReservedForGuest;
}
