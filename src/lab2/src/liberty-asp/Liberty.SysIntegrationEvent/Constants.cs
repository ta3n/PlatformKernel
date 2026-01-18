namespace Liberty.SysIntegrationEvent;

/// <summary>
/// Provides constants representing the names of various message queues
/// used within the reservation system for communication and processing purposes.
/// </summary>
public static class ReservationQueues
{
    /// <summary>
    /// Represents the message queue name used for processing reminders of upcoming check-in dates.
    /// This constant is utilized for configuring message consumers and endpoints in the system's
    /// message broker, managing the distribution of tasks related to notifying customers about their
    /// upcoming check-in schedules.
    /// </summary>
    public const string BookingReminderOfUpcomingCheckInDateQueue = nameof(BookingReminderOfUpcomingCheckInDateQueue);

    /// <summary>
    /// Represents the queue name used for processing events related to the booking cancellation fee reminders.
    /// It is utilized by the message broker to route messages to the appropriate endpoint
    /// responsible for handling scenarios where a reminder for a cancellation fee needs to be sent.
    /// This queue is configured as durable and supports message retry on failure to ensure reliable processing.
    /// </summary>
    public const string BookingCancellationFeeReminderQueue = nameof(BookingCancellationFeeReminderQueue);

    /// <summary>
    /// Represents the message queue for sending email reminders about upcoming check-in dates for bookings.
    /// This queue is consumed by the `BookingReminderOfUpcomingCheckInDateSendMailConsumer`
    /// to process and handle the sending of notification emails to users regarding upcoming check-ins.
    /// Primarily used in conjunction with MassTransit for distributed message handling.
    /// </summary>
    public const string BookingReminderOfUpcomingCheckInDateSendMailQueue = nameof(BookingReminderOfUpcomingCheckInDateSendMailQueue);

    /// <summary>
    /// Specifies the queue name for handling email notifications related to booking cancellation fee reminders.
    /// It is used as an endpoint in the messaging system to process and send such reminder emails.
    /// </summary>
    public const string BookingCancellationFeeReminderSendMailQueue = nameof(BookingCancellationFeeReminderSendMailQueue);

    /// <summary>
    /// Represents the queue name for sending email reminders about booking offer letters.
    /// This queue is used in the system to manage and process tasks related to notifying users about
    /// booking offer letters through email communication.
    /// </summary>
    public const string BookingReminderOfferLetterSendMailQueue = nameof(BookingReminderOfferLetterSendMailQueue);

    /// <summary>
    /// Represents the queue name for sending emails related to cancellation reminders
    /// of bookings. This constant is typically used for integration with messaging
    /// systems to handle email notifications for users about booking cancellations.
    /// </summary>
    public const string BookingReminderCancellationSendMailQueue = nameof(BookingReminderCancellationSendMailQueue);

    /// <summary>
    /// Represents the name of the queue used for sending booking-related emails in the system.
    /// </summary>
    /// <remarks>
    /// This queue is utilized for processing events related to booking email notifications.
    /// The queue ensures reliability and durability in message handling, and it is configured
    /// to work with MassTransit for consuming booking send email events.
    /// </remarks>
    public const string BookingSendMailQueue = nameof(BookingSendMailQueue);

    /// <summary>
    /// Represents the queue name for handling image resize operations in the
    /// messaging system. This constant is typically used to define endpoints for
    /// message queue configurations in the application.
    /// </summary>
    public const string ImageResizeQueue = nameof(ImageResizeQueue);

    /// <summary>
    /// Represents the message queue name used for managing the precomputation of booking search data.
    /// This constant is leveraged for configuring message consumers and endpoints in the system's
    /// message broker, handling tasks related to optimizing booking search operations by precomputing
    /// necessary indexes or metadata.
    /// </summary>
    public const string BookingSearchPrecomputeQueue = nameof(BookingSearchPrecomputeQueue);

    /// <summary>
    /// Represents the message queue name used for managing the partitioning of booking search data.
    /// This constant is utilized for configuring message consumers and endpoints in the system's
    /// message broker, handling tasks related to dividing booking search operations into smaller,
    /// manageable partitions for optimized processing.
    /// </summary>
    public const string BookingSearchPrecomputePartitionQueue = nameof(BookingSearchPrecomputePartitionQueue);

    /// <summary>
    /// Represents the message queue used for handling the upsert operations of room group application date data.
    /// This queue is utilized in queue configurations to ensure proper message routing for updating or adding
    /// room group-related application date records in the system.
    /// </summary>
    public const string RoomGroupAppDateUpsertQueue = nameof(RoomGroupAppDateUpsertQueue);

    /// <summary>
    /// Represents the message queue used for aggregating room group application date data.
    /// This queue is employed to ensure efficient and organized processing
    /// of aggregated data related to room group application dates.
    /// </summary>
    public const string RoomGroupAppDateAggregationQueue = nameof(RoomGroupAppDateAggregationQueue);

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

    /// <summary>
    /// Represents the message queue name used for processing audit logs of sent emails.
    /// This queue is utilized to track and log email dispatch events within the system.
    /// </summary>
    public const string EmailSentAuditLogQueue = nameof(EmailSentAuditLogQueue);

    /// <summary>
    /// Represents the message queue name used for logging Faximo booking events.
    /// This queue is utilized to route messages related to Faximo booking logs for further processing or auditing.
    /// </summary>
    public const string BookingAggregateFaxAuditLogQueue = nameof(BookingAggregateFaxAuditLogQueue);
}
