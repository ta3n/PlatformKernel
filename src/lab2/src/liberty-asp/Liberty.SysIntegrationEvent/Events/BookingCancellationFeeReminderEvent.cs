using Liberty.SysIntegrationEvent.Base;

namespace Liberty.SysIntegrationEvent.Events;

/// <summary>
/// Represents an event that triggers a reminder for booking cancellation fees.
/// </summary>
/// <remarks>
/// This class derives from <see cref="RecurringJobEvent"/> and is used to handle
/// recurring operations related to booking cancellation fee reminders. It inherits
/// the core properties and behavior for recurring job events, such as event type and name.
/// </remarks>
public class BookingCancellationFeeReminderEvent : RecurringJobEvent
{
    /// <summary>
    /// Represents an event triggered to remind about the cancellation fee for a booking.
    /// </summary>
    /// <remarks>
    /// This class is a specialization of the RecurringJobEvent and sets the event name
    /// explicitly to the name of the class. It is used to model events related to recurring
    /// reminders for booking cancellation fees.
    /// </remarks>
    public BookingCancellationFeeReminderEvent()
    {
        EventName = nameof(BookingCancellationFeeReminderEvent);
    }
}
