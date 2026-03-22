using MassTransit;
using SharedKernel.MassTransit.Test.Configuration;
using SharedKernel.MassTransit.Test.Contracts;
using SharedKernel.MassTransit.Test.Data;
using SharedKernel.MassTransit.Test.Domain;

namespace SharedKernel.MassTransit.Test.Consumers;

public sealed class ProcessPaymentConsumer(
    MassTransitTestDbContext dbContext,
    AppOptions appOptions
) : IConsumer<ProcessPayment>
{
    public async Task Consume(
        ConsumeContext<ProcessPayment> context
    )
    {
        var now = DateTime.UtcNow;

        dbContext.ProcessingLogs.Add(
            new ProcessingLog
            {
                Id = Guid.NewGuid(),
                OrderId = context.Message.CorrelationId,
                Step = "PaymentProcessed",
                Source = appOptions.Role,
                CreatedAtUtc = now
            }
        );

        await context.Publish(
            new PaymentProcessed(
                context.Message.CorrelationId,
                now
            ),
            context.CancellationToken
        );

        await dbContext.SaveChangesAsync(context.CancellationToken);
    }
}
