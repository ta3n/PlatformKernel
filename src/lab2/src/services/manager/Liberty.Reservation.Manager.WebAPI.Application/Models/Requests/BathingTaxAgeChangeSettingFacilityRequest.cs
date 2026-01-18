using System.Text.Json.Serialization;

namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record BathingTaxAgeChangeSettingFacilityRequest(
    [property: JsonRequired] bool UseSpaTax,
    string? SpaTaxComment,
    string? SpaTaxTable
);
