namespace SharedKernel.IntegrationEvent.Base;

/// <summary>
/// Represents an event for scheduling jobs within the system.
/// </summary>
/// <remarks>
/// The ScheduleJobEvent class extends the functionality of the BaseJobEvent class
/// and serves as a foundational component for specific job scheduling events.
/// This class initializes the EventType property to the name of the class, ensuring
/// accurate identification of the event type in the system.
/// </remarks>
public class ScheduleJobEvent : BaseJobEvent
{
    /// <summary>
    /// Represents an event used to schedule a job within the system.
    /// </summary>
    /// <remarks>
    /// This class inherits from <see cref="BaseJobEvent"/> and is specifically used
    /// for defining events related to job scheduling. It automatically sets the
    /// EventType property to the name of the class.
    /// </remarks>
    protected ScheduleJobEvent()
    {
        EventType = nameof(ScheduleJobEvent);
    }
}
