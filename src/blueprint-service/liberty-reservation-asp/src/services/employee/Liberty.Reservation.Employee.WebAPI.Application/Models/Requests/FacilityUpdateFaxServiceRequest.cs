using Newtonsoft.Json;

namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;

public record FacilityUpdateFaxServiceRequest(
    string? Fax,
    [property: JsonRequired] bool UseFax
)
{
    public long? Id { get; set; }
}
