namespace Liberty.Reservation.Application.Models.Requests;

public record BookingCancellationRequest
{
    public long? Id { get; set; }
}
