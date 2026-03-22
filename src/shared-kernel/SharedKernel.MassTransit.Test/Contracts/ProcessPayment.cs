namespace SharedKernel.MassTransit.Test.Contracts;

public sealed record ProcessPayment(
    Guid CorrelationId,
    decimal Amount
);
