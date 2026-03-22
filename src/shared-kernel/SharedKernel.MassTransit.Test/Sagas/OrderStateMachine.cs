using MassTransit;
using SharedKernel.MassTransit.Test.Contracts;

namespace SharedKernel.MassTransit.Test.Sagas;

public sealed class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    public State AwaitingInventory { get; private set; } = null!;

    public State AwaitingPayment { get; private set; } = null!;

    public State Completed { get; private set; } = null!;

    public Event<OrderSubmitted> OrderSubmitted { get; private set; } = null!;

    public Event<InventoryReserved> InventoryReserved { get; private set; } = null!;

    public Event<PaymentProcessed> PaymentProcessed { get; private set; } = null!;

    public OrderStateMachine()
    {
        InstanceState(state => state.CurrentState);

        Event(
            () => OrderSubmitted,
            definition =>
            {
                definition.CorrelateById(context => context.Message.CorrelationId);
                definition.InsertOnInitial = true;
                definition.SetSagaFactory(
                    context => new OrderState
                    {
                        CorrelationId = context.Message.CorrelationId,
                        CustomerId = context.Message.CustomerId,
                        Amount = context.Message.Amount,
                        SubmittedAtUtc = context.Message.SubmittedAtUtc
                    }
                );
            }
        );

        Event(
            () => InventoryReserved,
            definition => definition.CorrelateById(context => context.Message.CorrelationId)
        );

        Event(
            () => PaymentProcessed,
            definition => definition.CorrelateById(context => context.Message.CorrelationId)
        );

        Initially(
            When(OrderSubmitted)
                .Then(
                    context =>
                    {
                        context.Saga.CustomerId = context.Message.CustomerId;
                        context.Saga.Amount = context.Message.Amount;
                        context.Saga.SubmittedAtUtc = context.Message.SubmittedAtUtc;
                    }
                )
                .Publish(
                    context => new ReserveInventory(
                        context.Saga.CorrelationId,
                        context.Saga.CustomerId
                    )
                )
                .TransitionTo(AwaitingInventory)
        );

        During(
            AwaitingInventory,
            When(InventoryReserved)
                .Then(context => context.Saga.InventoryReservedAtUtc = context.Message.ReservedAtUtc)
                .Publish(
                    context => new ProcessPayment(
                        context.Saga.CorrelationId,
                        context.Saga.Amount
                    )
                )
                .TransitionTo(AwaitingPayment)
        );

        During(
            AwaitingPayment,
            When(PaymentProcessed)
                .Then(context => context.Saga.PaymentProcessedAtUtc = context.Message.ProcessedAtUtc)
                .Publish(
                    context => new OrderCompleted(
                        context.Saga.CorrelationId,
                        context.Message.ProcessedAtUtc
                    )
                )
                .TransitionTo(Completed)
        );
    }
}
