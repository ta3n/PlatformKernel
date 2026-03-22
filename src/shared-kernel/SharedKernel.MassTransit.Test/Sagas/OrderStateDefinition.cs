using MassTransit;

namespace SharedKernel.MassTransit.Test.Sagas;

public sealed class OrderStateDefinition : SagaDefinition<OrderState>
{
    public OrderStateDefinition()
    {
        EndpointName = "shared-kernel-mass-transit-test-order-saga";
        ConcurrentMessageLimit = 1;
    }

    protected override void ConfigureSaga(
        IReceiveEndpointConfigurator endpointConfigurator,
        ISagaConfigurator<OrderState> sagaConfigurator,
        IRegistrationContext context
    )
    {
        endpointConfigurator.UseMessageRetry(retry => retry.Interval(3, TimeSpan.FromSeconds(1)));
    }
}
