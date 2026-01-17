namespace SharedKernel.MassTransit.Options;

/// <summary>
/// Defines the configuration options for setting up a message queue system,
/// including the queue type and specific settings for RabbitMQ.
/// </summary>
public record MessageQueueOptions(
    string QueueType,
    RabbitMqOptions? RabbitMqOptions
);
