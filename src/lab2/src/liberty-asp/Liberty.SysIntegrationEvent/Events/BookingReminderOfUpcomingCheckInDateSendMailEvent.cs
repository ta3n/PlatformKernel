using Liberty.SysIntegrationEvent.Base;

namespace Liberty.SysIntegrationEvent.Events;

/// <summary>
/// Represents an event to send an email reminder for an upcoming check-in date of a booking.
/// </summary>
/// <remarks>
/// This event is triggered as part of a scheduled job to notify users about the upcoming check-in date for their booking.
/// </remarks>
public class BookingReminderOfUpcomingCheckInDateSendMailEvent : ScheduleJobEvent
{
    /// <summary>
    /// Represents an event for sending a reminder email about an upcoming check-in date for a booking.
    /// </summary>
    /// <remarks>
    /// This class extends <see cref="ScheduleJobEvent"/> and is specifically used to define
    /// events related to sending reminder emails for bookings with upcoming check-in dates.
    /// It sets the EventName property to the name of the class to facilitate proper identification
    /// within the system.
    /// </remarks>
    public BookingReminderOfUpcomingCheckInDateSendMailEvent()
    {
        EventName = nameof(BookingReminderOfUpcomingCheckInDateSendMailEvent);
    }
}
