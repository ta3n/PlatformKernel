using System.Text.Json.Serialization;

namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;

public record AlertMessageUpdateRequest(
    string? Title,
    [property: JsonRequired] string Content,
    string? Icon,
    string? Color,
    [property: JsonRequired] bool IsEnabled
)
{
    public long? Id { get; set; }
};
