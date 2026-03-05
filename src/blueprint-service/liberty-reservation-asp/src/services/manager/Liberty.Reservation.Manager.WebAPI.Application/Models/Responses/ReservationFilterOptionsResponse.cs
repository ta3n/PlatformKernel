namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record ReservationFilterOptionsResponse(
    IEnumerable<ReservationStatus> ReservationStatuses,
    IEnumerable<PaymentTypes> PaymentTypes,
    IEnumerable<ItemOfFilterOption> Rooms,
    IEnumerable<ItemOfFilterOption> Sites
);

public record ItemOfFilterOption(
    long Id,
    string Name
);
