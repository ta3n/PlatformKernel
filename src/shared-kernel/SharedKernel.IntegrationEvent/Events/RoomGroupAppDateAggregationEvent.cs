using SharedKernel.IntegrationEvent.Base;

namespace SharedKernel.IntegrationEvent.Events;

/// <summary>
/// Represents an event used for aggregating room group application data by date.
/// </summary>
/// <remarks>
/// This event is part of the system integration and is typically triggered during processes
/// involving room adjustments and aggregations for hotel booking systems.
/// </remarks>
/// <example>
/// This event is used in conjunction with a message queue and consumed by a consumer to process
/// room data grouped by application date. It is serialized with necessary payload data like room
/// and user information before being sent.
/// </example>
public class RoomGroupAppDateAggregationEvent : ScheduleJobEvent
{
    /// <summary>
    /// Represents an integration event specifically designed for aggregating room group application data
    /// based on date information. This event inherits from <see cref="ScheduleJobEvent"/>.
    /// </summary>
    /// <remarks>
    /// The purpose of this event is to facilitate the process of serialization and transmission
    /// of aggregated room group data during system jobs or schedules.
    /// </remarks>
    public RoomGroupAppDateAggregationEvent()
    {
        EventName = nameof(RoomGroupAppDateAggregationEvent);
    }
}
