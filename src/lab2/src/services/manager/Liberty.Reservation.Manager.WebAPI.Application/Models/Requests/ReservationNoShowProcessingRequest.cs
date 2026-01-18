namespace Liberty.Reservation.Manager.WebAPI.Application.Models.Requests;

public record ReservationNoShowProcessingRequest(
    string Reason
)
{
    public long? Id { get; set; }
}
