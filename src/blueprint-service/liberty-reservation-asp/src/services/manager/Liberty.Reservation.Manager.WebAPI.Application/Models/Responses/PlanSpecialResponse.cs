namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;

public record PlanSpecialResponse
{
    public long Id { get; init; }
    public bool IsSecret { get; init; }
    public string? SecretWord { get; init; }
}
