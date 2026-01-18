namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;

public record FacilitySeedStatusResponse(
    long? Id,
    string? Code,
    string? Name,
    bool HasSeededPersonAgeTypeData
);
