namespace SharedKernel.MassTransit.Options;

/// <summary>
/// Represents the configuration parameters required for connecting to a Kafka cluster.
/// </summary>
public record KafkaOptions(
    string? Host
);
