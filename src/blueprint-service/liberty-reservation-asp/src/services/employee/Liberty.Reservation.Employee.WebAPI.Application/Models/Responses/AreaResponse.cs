namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;

public record AreaResponse(
    long Id,
    string Code,
    string Name,
    bool IsEnabled
);
