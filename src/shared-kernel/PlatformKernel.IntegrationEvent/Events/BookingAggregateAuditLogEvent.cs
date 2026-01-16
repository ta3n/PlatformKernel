using PlatformKernel.IntegrationEvent.Base;

namespace PlatformKernel.IntegrationEvent.Events;

/// <summary>
/// Represents an audit log event associated with the Booking Aggregate.
/// </summary>
/// <remarks>
/// The BookingAggregateAuditLogEvent inherits from <see cref="ScheduleJobEvent"/>
/// and is used within the system to capture and log events related to the
/// Booking Aggregate. It provides a concrete implementation of an event type
/// with the EventName property automatically set to the class name.
/// </remarks>
public class BookingAggregateAuditLogEvent : ScheduleJobEvent
{
    /// <summary>
    /// Represents an audit log event specific to a booking aggregate.
    /// </summary>
    /// <remarks>
    /// The BookingAggregateAuditLogEvent class inherits from <see cref="ScheduleJobEvent"/> and
    /// is used to define events related to the audit logging of booking aggregates.
    /// It initializes the EventName property to the name of the class, providing a clear
    /// identification for event handling in the system.
    /// </remarks>
    public BookingAggregateAuditLogEvent()
    {
        EventName = nameof(BookingAggregateAuditLogEvent);
    }
}
