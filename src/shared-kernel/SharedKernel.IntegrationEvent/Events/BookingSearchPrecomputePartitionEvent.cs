using SharedKernel.IntegrationEvent.Base;

namespace SharedKernel.IntegrationEvent.Events;

/// <summary>
/// Represents an event for precomputing partitions in booking search operations.
/// This event is used to trigger scheduled jobs related to partitioning booking search data,
/// optimizing the processing and handling of booking search tasks.
/// </summary>
public class BookingSearchPrecomputePartitionEvent : ScheduleJobEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BookingSearchPrecomputePartitionEvent"/> class.
    /// Sets the event name to <see cref="BookingSearchPrecomputeEvent"/>.
    /// </summary>
    public BookingSearchPrecomputePartitionEvent()
    {
        EventName = nameof(BookingSearchPrecomputeEvent);
    }
}
