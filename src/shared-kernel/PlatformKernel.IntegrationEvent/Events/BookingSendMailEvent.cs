using PlatformKernel.IntegrationEvent.Base;

namespace PlatformKernel.IntegrationEvent.Events;

/// <summary>
/// Represents an event used for sending booking emails within the system.
/// This event is primarily designed for integration with scheduled jobs
/// and is identified by the event name "BookingSendMailEvent".
/// Inherits from the <see cref="ScheduleJobEvent" /> class.
/// </summary>
public class BookingSendMailEvent : ScheduleJobEvent
{
    /// <summary>
    /// Represents an event for sending mail related to a booking.
    /// </summary>
    /// <remarks>
    /// The BookingSendMailEvent class is a specialized type of ScheduleJobEvent,
    /// designed to handle the scheduling of mail notifications for booking operations.
    /// </remarks>
    public BookingSendMailEvent()
    {
        EventName = nameof(BookingSendMailEvent);
    }
}
