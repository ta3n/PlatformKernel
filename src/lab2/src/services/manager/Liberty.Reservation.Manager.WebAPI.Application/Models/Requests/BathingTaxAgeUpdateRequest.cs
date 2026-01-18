using Newtonsoft.Json;

namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record BathingTaxAgeUpdateRequest(
    [property: JsonRequired] long Id,
    string? Name,
    int? AgeMax,
    int? AgeMin,
    [property: JsonRequired] bool IsEnabled,
    [property: JsonRequired] MetaOfBathingTaxAgeUpdateRequest Meta,
    [property: JsonRequired] List<SpaOfBathingTaxAgeUpdateRequest> Spas
);

public record MetaOfBathingTaxAgeUpdateRequest(
    string? GroupName,
    [property: JsonRequired] FoodBeds Food,
    [property: JsonRequired] FoodBeds Bed,
    [property: JsonRequired] PersonAgeGroups PersonAgeGroup
);

public record SpaOfBathingTaxAgeUpdateRequest(
    int? PriceMin,
    int? PriceMax,
    int? Tax
);
