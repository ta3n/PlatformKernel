namespace SharedKernel.MassTransit.Test.Contracts;

public sealed record PaymentProcessed(
    Guid CorrelationId,
    DateTime ProcessedAtUtc
);
