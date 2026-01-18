namespace Liberty.Reservation.Site.Public.WebAPI.Models.Requests;

public record BookingFacilityRequest(
    string FacilityCode,
    string SiteCode
);
