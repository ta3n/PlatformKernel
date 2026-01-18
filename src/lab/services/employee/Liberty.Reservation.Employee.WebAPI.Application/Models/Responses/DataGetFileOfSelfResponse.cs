namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;

public record DataGetFileOfSelfResponse
{
    public string? FileId { get; init; }
    public string? FileSecret { get; init; }
}
