namespace Liberty.Reservation.User.WebAPI.Application.Models.Responses;

public record PersonAgeTypeResponse(
    long Id,
    string? Name,
    bool IsMain,
    int? AgeMin,
    int? AgeMax
);
