using Liberty.SysIntegrationEvent.Base;

namespace Liberty.SysIntegrationEvent.Events;

/// <summary>
/// Represents an event to send a reminder about an upcoming check-in date for a booking.
/// </summary>
/// <remarks>
/// This event is part of the system's recurring job mechanism and is designed to notify
/// users about their upcoming check-in dates. It inherits from <see cref="RecurringJobEvent"/>
/// to leverage standardized recurring event functionality.
/// </remarks>
public class BookingReminderOfUpcomingCheckInDateEvent : RecurringJobEvent
{
    /// <summary>
    /// Represents an event that acts as a reminder for an upcoming check-in date
    /// related to a booking in the system.
    /// </summary>
    /// <remarks>
    /// This class is a specialization of the RecurringJobEvent and is used to handle events
    /// that notify about check-in date reminders for bookings. It initializes the event name
    /// to reflect its specific purpose.
    /// </remarks>
    public BookingReminderOfUpcomingCheckInDateEvent()
    {
        EventName = nameof(BookingReminderOfUpcomingCheckInDateEvent);
    }
}
