using Liberty.Entity;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.AggregateLogs;

public class BookingAggregateFaxAuditLog : BaseAuditLog
{
    public string? FaxNumber { get; set; }
    public string? Subject { get; set; }
    public string? Result { get; set; }
    public bool IsSuccess { get; set; }
    public string? Body { get; set; }
}
