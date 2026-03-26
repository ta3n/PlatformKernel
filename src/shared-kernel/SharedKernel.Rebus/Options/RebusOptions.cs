using SharedKernel.Rebus.Constants;

namespace SharedKernel.Rebus.Options;

public sealed class RebusOptions
{
    public const string SectionName = "Rebus";

    public string TransportType { get; set; } = RebusTransportType.RabbitMq;

    public string InputQueueName { get; set; } = string.Empty;

    public int NumberOfWorkers { get; set; } = 1;

    public int MaxParallelism { get; set; } = 8;

    public RebusRabbitMqOptions RabbitMq { get; set; } = new();
}
