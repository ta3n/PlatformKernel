namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record BedTypeResponse(
    long Id,
    string? Code,
    string? Name,
    BedTypeUnitTypes UnitType,
    bool IsEnabled
);
