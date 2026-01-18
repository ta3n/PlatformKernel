using Newtonsoft.Json;

namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record RoomGroupCreateRequest(
    string Name,
    string? Overview,
    string GroupName,
    [property: JsonRequired] int CapacityMin,
    [property: JsonRequired] int CapacityMax,
    [property: JsonRequired] int BaseNumber
);
