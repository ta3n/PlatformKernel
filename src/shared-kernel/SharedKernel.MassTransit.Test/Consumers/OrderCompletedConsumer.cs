using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedKernel.MassTransit.Test.Configuration;
using SharedKernel.MassTransit.Test.Contracts;
using SharedKernel.MassTransit.Test.Data;
using SharedKernel.MassTransit.Test.Domain;

namespace SharedKernel.MassTransit.Test.Consumers;

public sealed class OrderCompletedConsumer(
    MassTransitTestDbContext dbContext,
    AppOptions appOptions,
    ILogger<OrderCompletedConsumer> logger
) : IConsumer<OrderCompleted>
{
    public async Task Consume(
        ConsumeContext<OrderCompleted> context
    )
    {
        var order = await dbContext.Orders.SingleOrDefaultAsync(
            item => item.Id == context.Message.CorrelationId,
            context.CancellationToken
        );

        if (order is null)
        {
            logger.LogWarning(
                "Order {OrderId} was not found when processing OrderCompleted.",
                context.Message.CorrelationId
            );
            return;
        }

        order.Status = "Completed";
        order.UpdatedAtUtc = context.Message.CompletedAtUtc;

        dbContext.ProcessingLogs.Add(
            new ProcessingLog
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                Step = "OrderCompleted",
                Source = appOptions.Role,
                CreatedAtUtc = context.Message.CompletedAtUtc
            }
        );

        await dbContext.SaveChangesAsync(context.CancellationToken);
    }
}
