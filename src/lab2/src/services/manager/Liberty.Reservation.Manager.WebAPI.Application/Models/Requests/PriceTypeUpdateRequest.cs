namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record PriceTypeUpdateRequest(
    string? ShortName,
    string? Name,
    string? Color
)
{
    public long? Id { get; set; }
}
