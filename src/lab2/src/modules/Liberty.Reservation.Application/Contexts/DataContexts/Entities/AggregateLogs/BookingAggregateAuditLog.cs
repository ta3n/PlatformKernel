using Liberty.Entity;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.AggregateLogs;

public class BookingAggregateAuditLog : BaseAuditLog
{
    public long AggregateId { get; set; }
    public long FacilityId { get; set; }
    public long SiteId { get; set; }
}
