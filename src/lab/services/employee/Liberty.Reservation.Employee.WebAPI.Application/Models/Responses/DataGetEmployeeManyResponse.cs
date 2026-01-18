namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;

public record DataGetEmployeeManyResponse
{
    public string? Id { get; init; }
    public string? Name { get; init; }
    // public string? Kana { get; init; }
    public string? EMail { get; init; }
    public DataEmployeeMeta? Meta { get; init; }
}

public record DataEmployeeMeta
{
    public string? Kana { get; init; }
    public DateTime? BirthDay { get; init; }
}
