using System.Text.Json.Serialization;

namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record BathingTaxAgeChangeStatusRequest(
    [property: JsonRequired] long Id,
    [property: JsonRequired] bool IsEnable
);
