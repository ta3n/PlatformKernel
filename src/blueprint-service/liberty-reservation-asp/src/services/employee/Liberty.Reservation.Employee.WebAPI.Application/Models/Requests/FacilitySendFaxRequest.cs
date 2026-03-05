using Newtonsoft.Json;

namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;

public record FacilitySendFaxRequest(
    [property: JsonRequired] string FaxNumber,
    [property: JsonRequired] string Subject,
    [property: JsonRequired] string Body
)
{
    public long? Id { get; set; }
}
