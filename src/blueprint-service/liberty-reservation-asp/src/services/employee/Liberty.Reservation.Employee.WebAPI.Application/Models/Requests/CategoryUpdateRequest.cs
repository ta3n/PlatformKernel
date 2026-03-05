namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;

public record CategoryUpdateRequest(
    string? Name,
    string? Description
)
{
    public long? Id { get; set; }
}
