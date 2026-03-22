using SharedKernel.MassTransit.Constants;

namespace SharedKernel.MassTransit.Options;

/// <summary>
/// Defines the configuration options for setting up a message queue system,
/// including the queue type and transport-specific settings.
/// </summary>
public sealed record MessageQueueOptions
{
    public string QueueType { get; init; } = MessageQueueType.RabbitMq;

    public RabbitMqOptions? RabbitMqOptions { get; init; }

    public KafkaOptions? KafkaOptions { get; init; }

    public void Deconstruct(
        out string queueType,
        out RabbitMqOptions? rabbitMqOptions
    )
    {
        queueType = QueueType;
        rabbitMqOptions = RabbitMqOptions;
    }

    public void Deconstruct(
        out string queueType,
        out RabbitMqOptions? rabbitMqOptions,
        out KafkaOptions? kafkaOptions
    )
    {
        queueType = QueueType;
        rabbitMqOptions = RabbitMqOptions;
        kafkaOptions = KafkaOptions;
    }
}
