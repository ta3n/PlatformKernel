using Liberty.SysIntegrationEvent.Base;

namespace Liberty.SysIntegrationEvent.Events;

/// <summary>
/// Represents an event that logs the audit information for a sent fax.
/// </summary>
/// <remarks>
/// This event is triggered when a fax is successfully sent, and it logs the relevant audit details.
/// It inherits from <see cref="ScheduleJobEvent"/>.
/// </remarks>
public class BookingAggregateFaxAuditLogEvent : ScheduleJobEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BookingAggregateFaxAuditLogEvent"/> class.
    /// </summary>
    /// <remarks>
    /// Sets the <see cref="ScheduleJobEvent.EventName"/> property to the name of this event.
    /// </remarks>
    public BookingAggregateFaxAuditLogEvent()
    {
        EventName = nameof(BookingAggregateFaxAuditLogEvent);
    }
}
