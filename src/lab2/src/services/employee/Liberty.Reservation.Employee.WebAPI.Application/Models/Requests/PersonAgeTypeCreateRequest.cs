using System.Text.Json.Serialization;

namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;

public record PersonAgeTypeCreateRequest(
    string? Name,
    int? AgeMin,
    int? AgeMax,
    bool? IsEnabled,
    [property: JsonRequired] MetaOfBathingTaxAgeUpdateRequest Meta,
    [property: JsonRequired] List<SpaOfBathingTaxAgeUpdateRequest> Spas
);
