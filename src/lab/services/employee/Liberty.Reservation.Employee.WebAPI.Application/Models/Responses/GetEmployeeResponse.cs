namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;

public record GetEmployeeResponse
{
    public string? Name { get; init; }
    public string? Kana { get; init; }
    public string? EMail { get; init; }
}
