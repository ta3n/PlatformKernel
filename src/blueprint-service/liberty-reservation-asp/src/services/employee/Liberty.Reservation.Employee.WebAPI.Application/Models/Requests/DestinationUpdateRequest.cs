namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;

public record DestinationUpdateRequest(
    string Code,
    string? Name,
    string? ShortName,
    string? Url
)
{
    public long? Id { get; set; }
}
