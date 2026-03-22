namespace SharedKernel.MassTransit.Test.Contracts;

public sealed record OrderCompleted(
    Guid CorrelationId,
    DateTime CompletedAtUtc
);
