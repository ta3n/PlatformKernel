namespace SharedKernel.MassTransit.Test.Contracts;

public sealed record InventoryReserved(
    Guid CorrelationId,
    DateTime ReservedAtUtc
);
