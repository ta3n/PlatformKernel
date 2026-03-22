namespace SharedKernel.MassTransit.Test.Contracts;

public sealed record ReserveInventory(
    Guid CorrelationId,
    string CustomerId
);
