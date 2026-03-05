namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record AllergenResponse(
    long Id,
    string? Name,
    bool IsEnabled
);
