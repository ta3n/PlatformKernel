using System.Text.Json.Serialization;

namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;

public record PersonAgeTypeUpdateRequest(
    string? Name,
    int? AgeMin,
    int? AgeMax,
    bool? IsEnabled,
    [property: JsonRequired] MetaOfBathingTaxAgeUpdateRequest Meta,
    [property: JsonRequired] List<SpaOfBathingTaxAgeUpdateRequest> Spas
)
{
    public long? Id { get; set; }
}

public record MetaOfBathingTaxAgeUpdateRequest(
    string? GroupName,
    [property: JsonRequired] FoodBeds Food,
    [property: JsonRequired] FoodBeds Bed,
    [property: JsonRequired] long PersonAgeGroup
);

public record SpaOfBathingTaxAgeUpdateRequest(
    int? PriceMin,
    int? PriceMax,
    int? Tax
);
