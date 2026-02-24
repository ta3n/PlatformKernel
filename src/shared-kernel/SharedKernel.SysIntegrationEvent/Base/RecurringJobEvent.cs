namespace SharedKernel.SysIntegrationEvent.Base;

/// <summary>
/// Represents an event that is scheduled to occur at recurring intervals
/// within the system. Inherits common job event properties and behavior
/// from BaseJobEvent.
/// </summary>
/// <remarks>
/// This class acts as a base class for specific recurring job-related
/// events. It initializes the event type to "RecurringJobEvent."
/// </remarks>
public class RecurringJobEvent : BaseJobEvent
{
    /// <summary>
    /// Represents a recurring job event in the system.
    /// </summary>
    /// <remarks>
    /// This class is a specialization of the BaseJobEvent and is used to model events
    /// associated with recurring jobs. It includes functionality inherited from BaseJobEvent,
    /// such as JSON data serialization and deserialization, and assigns a default event type
    /// of "RecurringJobEvent".
    /// </remarks>
    protected RecurringJobEvent()
    {
        EventType = nameof(RecurringJobEvent);
    }
}
