using SharedKernel.IntegrationEvent.Base;

namespace SharedKernel.IntegrationEvent.Events;

/// <summary>
/// Represents an integration event for precomputing booking search data.
/// </summary>
/// <remarks>
/// This event is specifically used to trigger the precomputation of booking search data as part
/// of a recurring job. It inherits from the RecurringJobEvent base class and sets the event name
/// to the name of the event for consistent identification within the system.
/// </remarks>
public class BookingSearchPrecomputeEvent : RecurringJobEvent
{
    /// <summary>
    /// Represents an event for precomputing booking search data, which is
    /// part of a recurring job configuration.
    /// </summary>
    /// <remarks>
    /// This event is used to handle the preparation and processing of booking
    /// search data in advance. It ensures efficient data handling for recurring
    /// job execution by initializing the associated event name.
    /// </remarks>
    public BookingSearchPrecomputeEvent()
    {
        EventName = nameof(BookingSearchPrecomputeEvent);
    }
}
