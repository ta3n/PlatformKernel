using PlatformKernel.IntegrationEvent.Base;

namespace PlatformKernel.IntegrationEvent.Events;

/// <summary>
/// Represents an event to send a cancellation email for a booking reminder.
/// </summary>
/// <remarks>
/// This event is triggered when a booking reminder is cancelled, and it initiates the process
/// of sending an email notification. It inherits properties and functionalities from
/// the <see cref="ScheduleJobEvent"/> class to facilitate job scheduling behavior.
/// </remarks>
public class BookingReminderCancellationSendMailEvent : ScheduleJobEvent
{
    /// <summary>
    /// Represents an event for sending a cancellation email reminder for a booking.
    /// </summary>
    /// <remarks>
    /// The BookingReminderCancellationSendMailEvent class inherits from <see cref="ScheduleJobEvent"/>
    /// and is used to define specific behavior for handling cancellation email reminders
    /// associated with bookings. The EventName property is automatically set to the name of this event class.
    /// </remarks>
    public BookingReminderCancellationSendMailEvent()
    {
        EventName = nameof(BookingReminderCancellationSendMailEvent);
    }
}
