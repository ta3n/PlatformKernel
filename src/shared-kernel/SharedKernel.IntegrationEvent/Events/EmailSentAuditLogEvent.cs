using SharedKernel.IntegrationEvent.Base;

namespace SharedKernel.IntegrationEvent.Events;

/// <summary>
/// Represents an event that logs the audit information for a sent email.
/// </summary>
/// <remarks>
/// This event is triggered when an email is successfully sent, and it logs the relevant audit details.
/// It inherits from <see cref="ScheduleJobEvent"/>.
/// </remarks>
public class EmailSentAuditLogEvent : ScheduleJobEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmailSentAuditLogEvent"/> class.
    /// </summary>
    /// <remarks>
    /// Sets the <see cref="ScheduleJobEvent.EventName"/> property to the name of this event.
    /// </remarks>
    public EmailSentAuditLogEvent()
    {
        EventName = nameof(EmailSentAuditLogEvent);
    }
}
