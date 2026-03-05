namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;

public record AppDateTypeUpdateRequest(
    string Name,
    string ShortName,
    string Color,
    string? Description
)
{
    public long? Id { get; set; }
}
