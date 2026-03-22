using MassTransit;

namespace SharedKernel.MassTransit.Test.Sagas;

public sealed class OrderState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }

    public string CurrentState { get; set; } = null!;

    public string CustomerId { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public DateTime SubmittedAtUtc { get; set; }

    public DateTime? InventoryReservedAtUtc { get; set; }

    public DateTime? PaymentProcessedAtUtc { get; set; }
}
