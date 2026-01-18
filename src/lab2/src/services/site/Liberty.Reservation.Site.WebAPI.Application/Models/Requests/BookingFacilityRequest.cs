namespace Liberty.Reservation.Site.WebAPI.Application.Models.Requests;

public record BookingFacilityRequest(
    string FacilityCode,
    string SiteCode
);
