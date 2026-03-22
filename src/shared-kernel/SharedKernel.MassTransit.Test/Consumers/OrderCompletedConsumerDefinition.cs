using MassTransit;

namespace SharedKernel.MassTransit.Test.Consumers;

public sealed class OrderCompletedConsumerDefinition : ConsumerDefinition<OrderCompletedConsumer>
{
    public OrderCompletedConsumerDefinition()
    {
        EndpointName = "shared-kernel-mass-transit-test-order-completed";
        ConcurrentMessageLimit = 1;
    }

    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<OrderCompletedConsumer> consumerConfigurator,
        IRegistrationContext context
    )
    {
        endpointConfigurator.UseMessageRetry(retry => retry.Interval(3, TimeSpan.FromSeconds(1)));
    }
}
