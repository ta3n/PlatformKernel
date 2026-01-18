using Newtonsoft.Json;

namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record OptionItemCreateRequest(
    string Name,
    string? Description,
    [property: JsonRequired] int BaseNumber,
    [property: JsonRequired] int Price
);
