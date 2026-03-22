using MassTransit;

namespace SharedKernel.MassTransit.Test.Consumers;

public sealed class ReserveInventoryConsumerDefinition : ConsumerDefinition<ReserveInventoryConsumer>
{
    public ReserveInventoryConsumerDefinition()
    {
        EndpointName = "shared-kernel-mass-transit-test-reserve-inventory";
        ConcurrentMessageLimit = 1;
    }

    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<ReserveInventoryConsumer> consumerConfigurator,
        IRegistrationContext context
    )
    {
        endpointConfigurator.UseMessageRetry(retry => retry.Interval(3, TimeSpan.FromSeconds(1)));
    }
}
