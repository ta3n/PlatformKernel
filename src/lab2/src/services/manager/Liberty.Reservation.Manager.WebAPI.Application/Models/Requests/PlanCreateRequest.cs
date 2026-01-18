using System.Text.Json.Serialization;

namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record PlanCreateRequest(
    string? Name,
    string? Summary,
    bool? DayUse,
    [property: JsonRequired] PlanTypes PlanType
);
