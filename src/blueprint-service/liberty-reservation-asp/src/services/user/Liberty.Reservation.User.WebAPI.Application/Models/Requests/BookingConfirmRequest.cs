namespace Liberty.Reservation.User.WebAPI.Application.Models.Requests;

public record BookingConfirmRequest
{
    public long? Id { get; set; }
    public string? GuestCode { get; set; }
}
