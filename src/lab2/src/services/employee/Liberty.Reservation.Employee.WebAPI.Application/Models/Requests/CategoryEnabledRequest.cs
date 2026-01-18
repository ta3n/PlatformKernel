using Newtonsoft.Json;

namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;

public record CategoryEnabledRequest(
    [property: JsonRequired] bool IsEnabled
)
{
    public long? Id { get; set; }
}
