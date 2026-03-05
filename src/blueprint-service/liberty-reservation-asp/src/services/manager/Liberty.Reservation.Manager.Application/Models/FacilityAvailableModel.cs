using Liberty.Entity.ValueObjects;

namespace Liberty.Reservation.Manager.Application.Models;

public record FacilityAvailableModel(
    long Id,
    string? Code,
    MultilingualText? Name,
    IEnumerable<SiteOfFacilityAvailableModel>? Sites,
    IEnumerable<PersonAgeTypeOfFacilityAvailableModel>? PersonAgeTypes
);

public record SiteOfFacilityAvailableModel(
    long Id,
    string? Code,
    MultilingualText? Name,
    string? ShortName
);

public record PersonAgeTypeOfFacilityAvailableModel(
    long Id,
    string? Code,
    MultilingualText? Name,
    bool IsMain
);

public record FacilitySiteFlatAvailableModel(
    long FacilityId,
    string? FacilityCode,
    long SiteId,
    string? SiteCode,
    IEnumerable<PersonAgeTypeOfFacilityAvailableModel>? PersonAgeTypes
);
