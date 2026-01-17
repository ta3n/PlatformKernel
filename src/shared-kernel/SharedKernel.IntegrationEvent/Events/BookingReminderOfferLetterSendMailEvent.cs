using SharedKernel.IntegrationEvent.Base;

namespace SharedKernel.IntegrationEvent.Events;

/// <summary>
/// Represents an integration event used to send reminder emails
/// for offer letters related to booking in the system.
/// </summary>
/// <remarks>
/// The BookingReminderOfferLetterSendMailEvent class inherits from
/// <see cref="ScheduleJobEvent"/> to leverage the event scheduling
/// capabilities. This event is specifically designed to handle the
/// process of sending reminder emails concerning offer letters
/// associated with bookings.
/// </remarks>
public class BookingReminderOfferLetterSendMailEvent : ScheduleJobEvent
{
    /// <summary>
    /// Represents an event used to send a reminder email for booking offer letters.
    /// </summary>
    /// <remarks>
    /// This event is specifically designed for scheduling reminder emails associated
    /// with booking offer letters. It inherits from <see cref="ScheduleJobEvent"/>
    /// and sets the <c>EventName</c> property to match its type name. This ensures
    /// proper identification and handling of this event type within the system.
    /// </remarks>
    public BookingReminderOfferLetterSendMailEvent()
    {
        EventName = nameof(BookingReminderOfferLetterSendMailEvent);
    }
}
