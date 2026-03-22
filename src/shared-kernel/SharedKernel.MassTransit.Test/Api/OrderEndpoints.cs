using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedKernel.MassTransit.Test.Contracts;
using SharedKernel.MassTransit.Test.Data;
using SharedKernel.MassTransit.Test.Domain;

namespace SharedKernel.MassTransit.Test.Api;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(
        this IEndpointRouteBuilder endpoints
    )
    {
        endpoints.MapPost(
            "/orders",
            async Task<IResult> (
                CreateOrderRequest request,
                MassTransitTestDbContext dbContext,
                IPublishEndpoint publishEndpoint,
                CancellationToken cancellationToken
            ) =>
            {
                if (string.IsNullOrWhiteSpace(request.CustomerId))
                {
                    return Results.BadRequest(new { Error = "customerId is required." });
                }

                if (request.Amount <= 0)
                {
                    return Results.BadRequest(new { Error = "amount must be greater than 0." });
                }

                var now = DateTime.UtcNow;
                var order = new OrderEntity
                {
                    Id = Guid.NewGuid(),
                    CustomerId = request.CustomerId,
                    Amount = request.Amount,
                    Status = "Submitted",
                    CreatedAtUtc = now,
                    UpdatedAtUtc = now
                };

                dbContext.Orders.Add(order);

                await publishEndpoint.Publish(
                    new OrderSubmitted(
                        order.Id,
                        order.CustomerId,
                        order.Amount,
                        now
                    ),
                    cancellationToken
                );

                await dbContext.SaveChangesAsync(cancellationToken);

                return Results.Accepted(
                    $"/orders/{order.Id}",
                    new
                    {
                        order.Id,
                        order.Status
                    }
                );
            }
        );

        endpoints.MapPost(
            "/orders/fail-after-publish",
            async Task<IResult> (
                CreateOrderRequest request,
                MassTransitTestDbContext dbContext,
                IPublishEndpoint publishEndpoint,
                CancellationToken cancellationToken
            ) =>
            {
                var order = new OrderEntity
                {
                    Id = Guid.NewGuid(),
                    CustomerId = request.CustomerId,
                    Amount = request.Amount,
                    Status = "Submitted",
                    CreatedAtUtc = DateTime.UtcNow,
                    UpdatedAtUtc = DateTime.UtcNow
                };

                dbContext.Orders.Add(order);

                await publishEndpoint.Publish(
                    new OrderSubmitted(
                        order.Id,
                        order.CustomerId,
                        order.Amount,
                        DateTime.UtcNow
                    ),
                    cancellationToken
                );

                throw new InvalidOperationException(
                    "Simulated failure after publish but before SaveChanges to verify the bus outbox."
                );
            }
        );

        endpoints.MapGet(
            "/orders",
            async Task<IResult> (
                MassTransitTestDbContext dbContext,
                CancellationToken cancellationToken
            ) =>
            {
                var orders = await dbContext.Orders
                    .AsNoTracking()
                    .OrderByDescending(order => order.CreatedAtUtc)
                    .Select(
                        order => new
                        {
                            order.Id,
                            order.CustomerId,
                            order.Amount,
                            order.Status,
                            order.CreatedAtUtc,
                            order.UpdatedAtUtc
                        }
                    )
                    .ToListAsync(cancellationToken);

                return Results.Ok(orders);
            }
        );

        endpoints.MapGet(
            "/orders/{orderId:guid}",
            async Task<IResult> (
                Guid orderId,
                MassTransitTestDbContext dbContext,
                CancellationToken cancellationToken
            ) =>
            {
                var order = await dbContext.Orders
                    .AsNoTracking()
                    .Where(item => item.Id == orderId)
                    .Select(
                        item => new
                        {
                            item.Id,
                            item.CustomerId,
                            item.Amount,
                            item.Status,
                            item.CreatedAtUtc,
                            item.UpdatedAtUtc
                        }
                    )
                    .SingleOrDefaultAsync(cancellationToken);

                if (order is null)
                {
                    return Results.NotFound();
                }

                var saga = await dbContext.OrderStates
                    .AsNoTracking()
                    .Where(state => state.CorrelationId == orderId)
                    .Select(
                        state => new
                        {
                            state.CorrelationId,
                            state.CurrentState,
                            state.CustomerId,
                            state.Amount,
                            state.SubmittedAtUtc,
                            state.InventoryReservedAtUtc,
                            state.PaymentProcessedAtUtc
                        }
                    )
                    .SingleOrDefaultAsync(cancellationToken);

                var logs = await dbContext.ProcessingLogs
                    .AsNoTracking()
                    .Where(log => log.OrderId == orderId)
                    .OrderBy(log => log.CreatedAtUtc)
                    .Select(
                        log => new
                        {
                            log.Id,
                            log.Step,
                            log.Source,
                            log.CreatedAtUtc
                        }
                    )
                    .ToListAsync(cancellationToken);

                return Results.Ok(
                    new
                    {
                        Order = order,
                        Saga = saga,
                        Logs = logs
                    }
                );
            }
        );

        return endpoints;
    }
}
