namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record CancellationPolicyResponse(
    long Id,
    string? Name,
    bool IsEnabled,
    string? TableSource,
    string? Description
);
