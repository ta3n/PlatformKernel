using Newtonsoft.Json;

namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;

public record DestinationEnabledRequest(
    [property: JsonRequired] bool IsEnabled
)
{
    public long? Id { get; set; }
}
