using Newtonsoft.Json;

namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;

public record AreaUpdateRequest(
    [property: JsonRequired] string Name
)
{
    public long? Id { get; set; }
}
