namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;

public record DestinationResponse
{
    public long Id { get; init; }
    public string? Code { get; init; }
    public string? Name { get; init; }
    public string? ShortName { get; init; }
    public string? PrefixName { get; init; }
    public string? Url { get; init; }
    public long DisplayOrder { get; init; }
    public bool IsEnabled { get; init; }
}
