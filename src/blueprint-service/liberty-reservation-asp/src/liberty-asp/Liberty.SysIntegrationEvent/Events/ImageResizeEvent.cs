using Liberty.SysIntegrationEvent.Base;

namespace Liberty.SysIntegrationEvent.Events;

/// <summary>
/// Represents a message event used for scheduling image resizing jobs in the system.
/// </summary>
/// <remarks>
/// The <c>ImageResizeEvent</c> class inherits from the <c>ScheduleJobEvent</c>, which is a type of scheduled job event.
/// This event is primarily leveraged in scenarios requiring asynchronous processing for image resizing tasks.
/// It is integrated with message queue services (e.g., RabbitMQ) to facilitate effective event-driven architecture.
/// </remarks>
/// <example>
/// This event can be published via a message bus (such as MassTransit) and consumed by a consumer service
/// that processes the associated image resizing job.
/// </example>
public class ImageResizeEvent : ScheduleJobEvent
{
    /// <summary>
    /// Represents an event used for image resizing operations.
    /// </summary>
    /// <remarks>
    /// This event is triggered to handle image resizing tasks. It extends from <see cref="ScheduleJobEvent"/>
    /// and automatically sets its <c>EventName</c> property to the name of the event type.
    /// </remarks>
    public ImageResizeEvent()
    {
        EventName = nameof(ImageResizeEvent);
    }
}
