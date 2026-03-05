using System.Text.Json.Serialization;

namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record BathingTaxAgeCreateRequest(
    string? Name,
    int? AgeMax,
    int? AgeMin,
    long? DisplayOrder,
    [property: JsonRequired] bool IsEnabled,
    [property: JsonRequired] MetaOfBathingTaxAgeUpdateRequest Meta,
    [property: JsonRequired] List<SpaOfBathingTaxAgeUpdateRequest> Spas
);
