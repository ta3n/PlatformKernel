using Liberty.SysIntegrationEvent.Base;
using MassTransit;
using MassTransit.Internals;
using System.Text.Json;

namespace Liberty.Reservation.Booking.Worker.Consumers.Base;

/// <summary>
/// Represents a base consumer for handling messages in a MassTransit-based application.
/// </summary>
/// <typeparam name="TMessage">The type of the message being consumed, which must derive from BaseJobEvent.</typeparam>
/// <remarks>
/// BaseConsumer provides common functionality for logging and consuming messages.
/// Derived classes must implement the <see cref="ExecuteAsync" /> method to define
/// specific behavior for processing the consumed message.
/// </remarks>
public abstract class BaseConsumer<TMessage>(
    ILogger<BaseConsumer<TMessage>> logger
) : IConsumer<TMessage> where TMessage : BaseJobEvent
{
    /// <summary>
    /// Consumes a message of type <typeparamref name="TMessage"/> and logs useful information
    /// before executing business-specific logic.
    /// </summary>
    /// <typeparam name="TMessage">The message type being consumed.</typeparam>
    /// <param name="context">The context providing access to the consumed message and other contextual information.</param>
    /// <returns>A task that represents the asynchronous operation of consuming and processing the message.</returns>
    public virtual async Task Consume(
        ConsumeContext<TMessage> context
    )
    {
        var consumerName = typeof(BaseConsumer<TMessage>).GetTypeName();

        logger.LogInformation(
            "{ConsumerName} - Execute: {Message}",
            consumerName,
            JsonSerializer.Serialize(context.Message)
        );

        await ExecuteAsync(context);
    }

    /// <summary>
    /// Executes the custom logic for processing a consumed message in a derived consumer.
    /// This method must be implemented by derived classes to define specific handling logic.
    /// </summary>
    /// <param name="context">The context of the consumed message, which contains the message data and metadata.</param>
    /// <returns>A task that represents the asynchronous operation of processing the message.</returns>
    protected abstract Task ExecuteAsync(
        ConsumeContext<TMessage> context
    );
}
