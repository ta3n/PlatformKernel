namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;

public record ConsumptionTaxResponse(
    long Id,
    string Code,
    string? Name,
    float Rate,
    long EnabledStart,
    long EnabledEnd
);
