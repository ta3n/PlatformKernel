using SharedKernel.IntegrationEvent.Base;

namespace SharedKernel.IntegrationEvent.Events;

/// <summary>
/// Represents an event that triggers the reminder email for a booking cancellation fee.
/// </summary>
/// <remarks>
/// This event is used in the scheduling system to notify users about cancellation fee reminders
/// via email. It extends the <see cref="ScheduleJobEvent"/> class to integrate with the job
/// scheduling infrastructure. The event name is automatically assigned based on its class name.
/// </remarks>
/// <example>
/// This class is typically consumed by handlers and consumers, such as
/// <c>BookingCancellationFeeReminderSendMailConsumer</c>, to process and send the appropriate
/// notification emails. It is registered as a scheduled job during the booking reservation
/// cancellation workflow.
/// </example>
public class BookingCancellationFeeReminderSendMailEvent : ScheduleJobEvent
{
    /// <summary>
    /// Represents a scheduled job event to send a booking cancellation fee reminder email.
    /// </summary>
    /// <remarks>
    /// This class is a specific implementation of <see cref="ScheduleJobEvent"/>
    /// utilized for triggering email reminders related to booking cancellation fees.
    /// It sets the <see cref="BaseJobEvent.EventName"/> property to the class name for event identification purposes.
    /// </remarks>
    public BookingCancellationFeeReminderSendMailEvent()
    {
        EventName = nameof(BookingCancellationFeeReminderSendMailEvent);
    }
}
