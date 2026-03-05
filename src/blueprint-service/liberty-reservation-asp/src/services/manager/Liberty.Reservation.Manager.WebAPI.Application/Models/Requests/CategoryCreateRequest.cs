namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record CategoryCreateRequest(
    string? Name,
    string? Description
);
