using Liberty.Entity.ValueObjects;

namespace Liberty.Reservation.Employee.Application.Models;

public record FacilitySeedDataStatus(
    long? Id,
    string? Code,
    MultilingualText? Name,
    bool HasSeededPersonAgeTypeData
);
