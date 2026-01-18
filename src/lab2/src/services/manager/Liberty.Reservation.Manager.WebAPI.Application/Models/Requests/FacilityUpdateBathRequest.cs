namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record FacilityUpdateBathRequest(
    string? SpaType,
    string? SpaName,
    string? SpaInfoComment,
    string? SpaDescription
);
