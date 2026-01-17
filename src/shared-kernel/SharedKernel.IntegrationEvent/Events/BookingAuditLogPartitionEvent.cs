using SharedKernel.IntegrationEvent.Base;

namespace SharedKernel.IntegrationEvent.Events;

/// <summary>
/// Represents an event associated with the partitioning of booking audit logs.
/// </summary>
/// <remarks>
/// The BookingAuditLogPartitionEvent class is a specific type of scheduled job event
/// within the system. It inherits from <see cref="ScheduleJobEvent"/> and is used to
/// initiate or reference tasks related to creating partitions for booking audit logs.
/// Upon instantiation, the EventName property is automatically set to the name of the class.
/// </remarks>
public class BookingAuditLogPartitionEvent : ScheduleJobEvent
{
    /// <summary>
    /// Represents an event for creating audit log partitions related to bookings in the system.
    /// </summary>
    /// <remarks>
    /// This class extends the <see cref="ScheduleJobEvent"/> class and is specifically
    /// used to define job scheduling events for partitioning booking audit logs.
    /// The <c>EventName</c> property is automatically set to the name of the class.
    /// </remarks>
    public BookingAuditLogPartitionEvent()
    {
        EventName = nameof(BookingAuditLogPartitionEvent);
    }
}
