using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.MassTransit;
using SharedKernel.MassTransit.Constants;
using SharedKernel.MassTransit.Options;

namespace SharedKernel.MassTransit.Test;

public class UnitTest1
{
    [Fact]
    public void MessageQueueOptions_DeconstructsConfiguredValues()
    {
        var options = new MessageQueueOptions
        {
            QueueType = MessageQueueType.Kafka,
            RabbitMqOptions = new RabbitMqOptions("amqp://localhost", "user", "pass"),
            KafkaOptions = new KafkaOptions("localhost:9092")
        };

        var (queueType, rabbitMqOptions, kafkaOptions) = options;

        Assert.Equal(MessageQueueType.Kafka, queueType);
        Assert.Equal("amqp://localhost", rabbitMqOptions!.Url);
        Assert.Equal("localhost:9092", kafkaOptions!.Host);
    }

    [Fact]
    public void AddMassTransitCustom_ThrowsForUnsupportedQueueType()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
            [
                new KeyValuePair<string, string?>("MessageQueueSettings:QueueType", "Unsupported")
            ])
            .Build();

        var exception = Assert.Throws<NotSupportedException>(() => services.AddMassTransitCustom(configuration));

        Assert.Contains("Unsupported", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void MessageQueueType_ExposesExpectedConstants()
    {
        Assert.Equal("RabbitMq", MessageQueueType.RabbitMq);
        Assert.Equal("Kafka", MessageQueueType.Kafka);
    }
}
