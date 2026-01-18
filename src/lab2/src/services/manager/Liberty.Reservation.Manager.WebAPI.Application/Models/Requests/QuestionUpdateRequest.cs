using Newtonsoft.Json;

namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record QuestionUpdateRequest(
    string? Name,
    string? Description,
    string? FormData,
    [property: JsonRequired] QuestionTypes Type,
    [property: JsonRequired] bool IsRequired
)
{
    public long? Id { get; set; }
}
