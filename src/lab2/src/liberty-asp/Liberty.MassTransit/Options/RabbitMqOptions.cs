namespace Liberty.MassTransit.Options;

/// <summary>
/// Represents the configuration parameters required for connecting to a RabbitMQ server.
/// </summary>
public record RabbitMqOptions(
    string? Url,
    string? Username,
    string? Password
);
