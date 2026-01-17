using SharedKernel.IntegrationEvent.Base;

namespace SharedKernel.IntegrationEvent.Events;

/// <summary>
/// Represents an event used to perform a bulk upsert operation for Room Group App Date.
/// </summary>
/// <remarks>
/// This event is typically produced when there is a need to update or insert a bulk set of Room Group App Date entries.
/// It serves as a message that can be published and consumed by various services or components in the system.
/// </remarks>
/// <example>
/// This class can be used in conjunction with a message bus to enable asynchronous communication
/// between producers and consumers, facilitating the bulk handling of Room Group App Date information.
/// </example>
/// <seealso cref="ScheduleJobEvent"/>
public class RoomGroupAppDateBulkUpsertEvent : ScheduleJobEvent
{
    /// <summary>
    /// Represents an event used for performing bulk upsert operations on room group application dates.
    /// </summary>
    /// <remarks>
    /// This event is typically utilized in scenarios such as caching, scheduling, or aggregating room group application date data.
    /// It inherits from <see cref="ScheduleJobEvent"/> which provides base scheduling-related functionality.
    /// </remarks>
    public RoomGroupAppDateBulkUpsertEvent()
    {
        EventName = nameof(RoomGroupAppDateBulkUpsertEvent);
    }
}
