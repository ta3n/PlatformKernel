using System.Text.Json.Serialization;

namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record BathingTaxAgeVisibleRequest(
    [property: JsonRequired] long Id,
    [property: JsonRequired] bool IsVisible
);
