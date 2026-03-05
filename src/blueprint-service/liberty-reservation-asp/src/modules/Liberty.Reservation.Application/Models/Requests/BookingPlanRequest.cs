namespace Liberty.Reservation.Application.Models.Requests;

public record BookingPlanRequest(
    long FacilityId,
    long SiteId,
    long ReservationDate,
    int RestNumber,
    string? Secret
);
