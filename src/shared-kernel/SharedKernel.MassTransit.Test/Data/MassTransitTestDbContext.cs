using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using SharedKernel.MassTransit.Test.Domain;
using SharedKernel.MassTransit.Test.Sagas;

namespace SharedKernel.MassTransit.Test.Data;

public sealed class MassTransitTestDbContext(
    DbContextOptions<MassTransitTestDbContext> options
) : SagaDbContext(options)
{
    public DbSet<OrderEntity> Orders => Set<OrderEntity>();

    public DbSet<ProcessingLog> ProcessingLogs => Set<ProcessingLog>();

    public DbSet<OrderState> OrderStates => Set<OrderState>();

    protected override IEnumerable<ISagaClassMap> Configurations
    {
        get
        {
            yield return new OrderStateMap();
        }
    }

    protected override void OnModelCreating(
        ModelBuilder modelBuilder
    )
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MassTransitTestDbContext).Assembly);
        modelBuilder.AddMassTransitTransactionalOutboxEntities();
    }
}
