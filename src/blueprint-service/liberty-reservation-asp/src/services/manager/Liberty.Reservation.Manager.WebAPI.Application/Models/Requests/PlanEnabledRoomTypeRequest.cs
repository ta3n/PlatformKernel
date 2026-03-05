using Newtonsoft.Json;

namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record PlanEnabledRoomTypeRequest(
    [property: JsonRequired] bool IsEnabled
);
