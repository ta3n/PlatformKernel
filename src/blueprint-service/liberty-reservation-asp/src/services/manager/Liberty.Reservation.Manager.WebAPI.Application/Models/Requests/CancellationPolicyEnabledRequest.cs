using Newtonsoft.Json;

namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record CancellationPolicyEnabledRequest(
    [property: JsonRequired] bool IsEnabled
);
