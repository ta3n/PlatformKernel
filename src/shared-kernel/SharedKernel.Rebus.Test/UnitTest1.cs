using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SharedKernel.Rebus;
using SharedKernel.Rebus.Constants;
using SharedKernel.Rebus.Options;
using Xunit;

namespace SharedKernel.Rebus.Test;

public class UnitTest1
{
    [Fact]
    public void RebusTransportType_ExposesExpectedConstants()
    {
        Assert.Equal("InMemory", RebusTransportType.InMemory);
        Assert.Equal("RabbitMq", RebusTransportType.RabbitMq);
    }

    [Fact]
    public void AddRebusCustom_ThrowsWhenSectionMissing()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        var exception = Assert.Throws<InvalidOperationException>(
            () => services.AddRebusCustom(configuration)
        );

        Assert.Contains(RebusOptions.SectionName, exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void AddRebusCustom_ThrowsForUnsupportedTransportType()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
            [
                new KeyValuePair<string, string?>("Rebus:TransportType", "Kafka"),
                new KeyValuePair<string, string?>("Rebus:InputQueueName", "shared-kernel-rebus-test"),
                new KeyValuePair<string, string?>("Rebus:RabbitMq:ConnectionString", "amqp://guest:guest@localhost:5672")
            ]
            )
            .Build();

        var exception = Assert.Throws<NotSupportedException>(
            () => services.AddRebusCustom(configuration)
        );

        Assert.Contains("Kafka", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void AddRebusCustom_RegistersHostedService_AndBindsOptions()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
            [
                new KeyValuePair<string, string?>("Rebus:TransportType", RebusTransportType.InMemory),
                new KeyValuePair<string, string?>("Rebus:InputQueueName", "shared-kernel-rebus-test"),
                new KeyValuePair<string, string?>("Rebus:NumberOfWorkers", "2"),
                new KeyValuePair<string, string?>("Rebus:MaxParallelism", "4")
            ]
            )
            .Build();

        services.AddRebusCustom(configuration);

        Assert.Contains(
            services,
            descriptor => descriptor.ServiceType == typeof(IHostedService)
        );

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<RebusOptions>>().Value;

        Assert.Equal(RebusTransportType.InMemory, options.TransportType);
        Assert.Equal("shared-kernel-rebus-test", options.InputQueueName);
        Assert.Equal(2, options.NumberOfWorkers);
        Assert.Equal(4, options.MaxParallelism);
    }
}
