namespace Liberty.Reservation.Application.Models.Requests;

public record BookingPlanDetailRequest(
    long FacilityId,
    long SiteId,
    long PlanId,
    long RoomGroupId,
    long ReservationDate,
    int RestNumber,
    string? Secret
);
