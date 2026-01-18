namespace Liberty.Reservation.Application.Models.Requests;

public record BookingCancellationByManagerRequest(
    decimal? CancellationFee
)
{
    public long? Id { get; set; }
};
