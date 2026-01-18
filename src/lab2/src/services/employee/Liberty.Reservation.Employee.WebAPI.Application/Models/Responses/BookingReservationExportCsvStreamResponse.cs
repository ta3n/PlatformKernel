namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;

public class BookingReservationExportCsvStreamResponse
{
    public Func<Stream, CancellationToken, Task> WriteToStreamAsync { get; set; } = null!;
}
