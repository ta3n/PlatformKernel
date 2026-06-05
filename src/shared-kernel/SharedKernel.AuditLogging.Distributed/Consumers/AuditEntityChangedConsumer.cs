using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedKernel.AuditLogging.Distributed.Contracts;
using SharedKernel.AuditLogging.Distributed.Services;

namespace SharedKernel.AuditLogging.Distributed.Consumers;

public sealed class AuditEntityChangedConsumer<TDbContext>(
    TDbContext dbContext,
    AuditEntityChangedProcessor processor
) : IConsumer<AuditEntityChanged> where TDbContext : DbContext
{
    public async Task Consume(
        ConsumeContext<AuditEntityChanged> context
    )
    {
        await processor.ProcessAsync(dbContext, context.Message, context.CancellationToken);
    }
}
