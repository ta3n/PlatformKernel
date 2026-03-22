namespace SharedKernel.MassTransit.Test.Api;

public sealed record CreateOrderRequest(
    string CustomerId,
    decimal Amount
);
