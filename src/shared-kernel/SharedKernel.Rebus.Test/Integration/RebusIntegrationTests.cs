using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Handlers;
using Rebus.Routing.TypeBased;
using SharedKernel.Rebus.Constants;
using Xunit;

namespace SharedKernel.Rebus.Test.Integration;

[Collection(RabbitMqIntegrationCollection.CollectionName)]
public sealed class RebusIntegrationTests(
    RabbitMqContainerFixture fixture
)
{
    [Fact]
    [Trait("Category", "AppHost")]
    public async Task Publish_DeliversSubscribedEvent_ThroughRabbitMq()
    {
        var queueName = $"shared-kernel-rebus-publish-{Guid.NewGuid():N}";
        var probe = new TestMessageProbe<TestEvent>();
        using var host = await CreateHostAsync(
            queueName,
            probe,
            onCreated: bus => bus.Subscribe<TestEvent>()
        );

        var bus = host.Services.GetRequiredService<IBus>();
        var message = new TestEvent(Guid.NewGuid().ToString("N"));

        await bus.Publish(message);

        var received = await probe.WaitAsync();

        Assert.Equal(message.MessageId, received.MessageId);
        await host.StopAsync();
    }

    [Fact]
    public async Task Send_DeliversCommand_WhenRoutingIsConfigured()
    {
        var queueName = $"shared-kernel-rebus-send-{Guid.NewGuid():N}";
        var probe = new TestMessageProbe<TestCommand>();
        using var host = await CreateHostAsync(
            queueName,
            probe,
            configure: rebus => rebus.Routing(
                routing => routing.TypeBased().Map<TestCommand>(queueName)
            )
        );

        var bus = host.Services.GetRequiredService<IBus>();
        var message = new TestCommand(Guid.NewGuid().ToString("N"));

        await bus.Send(message);

        var received = await probe.WaitAsync();

        Assert.Equal(message.MessageId, received.MessageId);
        await host.StopAsync();
    }

    private async Task<IHost> CreateHostAsync<TMessage>(
        string queueName,
        TestMessageProbe<TMessage> probe,
        Func<RebusConfigurer, RebusConfigurer>? configure = null,
        Func<IBus, Task>? onCreated = null
    )
        where TMessage : class
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
            [
                new KeyValuePair<string, string?>("Rebus:TransportType", RebusTransportType.RabbitMq),
                new KeyValuePair<string, string?>("Rebus:InputQueueName", queueName),
                new KeyValuePair<string, string?>("Rebus:NumberOfWorkers", "1"),
                new KeyValuePair<string, string?>("Rebus:MaxParallelism", "1"),
                new KeyValuePair<string, string?>("Rebus:RabbitMq:ConnectionString", fixture.ConnectionString)
            ]
            )
            .Build();

        var host = Host.CreateDefaultBuilder()
            .ConfigureServices(
                services =>
                {
                    services.AddSingleton(probe);
                    services.AddSingleton<IHandleMessages<TMessage>, ProbeMessageHandler<TMessage>>();
                    services.AddRebusCustom(
                        configuration,
                        configure,
                        onCreated
                    );
                }
            )
            .Build();

        await host.StartAsync();

        return host;
    }

    public sealed record TestEvent(string MessageId);

    public sealed record TestCommand(string MessageId);

    private sealed class ProbeMessageHandler<TMessage>(
        TestMessageProbe<TMessage> probe
    ) : IHandleMessages<TMessage>
        where TMessage : class
    {
        public Task Handle(
            TMessage message
        )
        {
            probe.Record(message);
            return Task.CompletedTask;
        }
    }

    private sealed class TestMessageProbe<TMessage>
        where TMessage : class
    {
        private readonly TaskCompletionSource<TMessage> _completionSource = new(
            TaskCreationOptions.RunContinuationsAsynchronously
        );

        public void Record(
            TMessage message
        )
        {
            _completionSource.TrySetResult(message);
        }

        public async Task<TMessage> WaitAsync(
            TimeSpan? timeout = null
        )
        {
            var timeoutTask = Task.Delay(timeout ?? TimeSpan.FromSeconds(15));
            var completedTask = await Task.WhenAny(
                _completionSource.Task,
                timeoutTask
            );

            if (completedTask != _completionSource.Task)
            {
                throw new TimeoutException($"Timed out waiting for {typeof(TMessage).Name}.");
            }

            return await _completionSource.Task;
        }
    }
}
