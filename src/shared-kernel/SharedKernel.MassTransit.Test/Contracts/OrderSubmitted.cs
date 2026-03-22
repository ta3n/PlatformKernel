namespace SharedKernel.MassTransit.Test.Contracts;

public sealed record OrderSubmitted(
    Guid CorrelationId,
    string CustomerId,
    decimal Amount,
    DateTime SubmittedAtUtc
);
