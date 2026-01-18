namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;

public record ReservationFilterOptionsResponse(
    IEnumerable<ReservationStatus> ReservationStatuses,
    IEnumerable<PaymentTypes> PaymentTypes
);
