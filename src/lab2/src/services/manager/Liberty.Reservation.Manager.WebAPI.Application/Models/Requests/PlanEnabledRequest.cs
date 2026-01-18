using Newtonsoft.Json;

namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record PlanEnabledRequest(
    [property: JsonRequired] bool IsEnabled
);
