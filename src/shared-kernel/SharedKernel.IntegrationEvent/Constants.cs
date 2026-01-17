namespace SharedKernel.IntegrationEvent;

/// <summary>
/// Provides constants representing the names of various message queues
/// used within the reservation system for communication and processing purposes.
/// </summary>
public static class AppQueues
{
    /// <summary>
    /// Specifies the message queue name used for handling the aggregation of booking-related audit logs.
    /// This constant is employed to configure message consumers and endpoints within the messaging system,
    /// facilitating the organization and processing of audit data associated with booking activities.
    /// </summary>
    public const string BookingAggregationAuditLogQueue = nameof(BookingAggregationAuditLogQueue);

    /// <summary>
    /// Specifies the message queue name used for partitioning booking audit logs.
    /// It helps in organizing and processing detailed audit information for bookings.
    /// </summary>
    public const string BookingAuditLogPartitionQueue = nameof(BookingAuditLogPartitionQueue);
}
