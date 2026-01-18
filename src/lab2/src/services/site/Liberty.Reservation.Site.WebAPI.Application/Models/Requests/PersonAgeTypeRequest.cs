namespace Liberty.Reservation.Site.WebAPI.Application.Models.Requests;

public record PersonAgeTypeRequest
{
    public long Id { get; init; }
    public bool IsBed { get; init; }
    public bool IsFood { get; init; }
    public bool IsMain { get; init; }
    public string? Name { get; init; }
}
