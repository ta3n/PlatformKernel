namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record CancellationPolicyCreateRequest(
    string? Name,
    string? Description
);
