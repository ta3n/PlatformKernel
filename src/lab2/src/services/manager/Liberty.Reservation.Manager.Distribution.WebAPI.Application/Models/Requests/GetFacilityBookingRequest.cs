namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests;

public record GetFacilityBookingRequest(
    string FacilityCode,
    string SiteCode
);
