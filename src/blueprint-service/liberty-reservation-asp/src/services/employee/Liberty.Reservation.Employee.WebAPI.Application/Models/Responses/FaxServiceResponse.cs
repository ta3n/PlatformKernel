namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;

public record FaxServiceResponse(
    long Id,
    string? Code,
    string? Name,
    bool IsEnabled
);
