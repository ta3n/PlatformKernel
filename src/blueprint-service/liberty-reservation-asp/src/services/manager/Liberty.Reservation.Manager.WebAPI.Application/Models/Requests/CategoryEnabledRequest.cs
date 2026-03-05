using Newtonsoft.Json;

namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record CategoryEnabledRequest(
    [property: JsonRequired] bool IsEnabled
)
{
    public long? Id { get; set; }
}
