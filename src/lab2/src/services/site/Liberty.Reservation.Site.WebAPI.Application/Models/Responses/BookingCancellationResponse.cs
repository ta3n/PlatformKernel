namespace Liberty.Reservation.Site.WebAPI.Application.Models.Responses;

public record BookingCancellationResponse
{
    public string? Code { get; set; }
    public string? Name {get; set; }
    public string? Description { get; set; }
    public string? TableSource { get; set; }
}
