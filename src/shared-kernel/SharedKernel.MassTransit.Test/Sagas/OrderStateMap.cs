using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SharedKernel.MassTransit.Test.Sagas;

public sealed class OrderStateMap : SagaClassMap<OrderState>
{
    protected override void Configure(
        EntityTypeBuilder<OrderState> entity,
        ModelBuilder model
    )
    {
        entity.ToTable("test_order_saga_states");

        entity.Property(item => item.CurrentState)
            .HasMaxLength(64);

        entity.Property(item => item.CustomerId)
            .HasMaxLength(128);

        entity.Property(item => item.Amount)
            .HasPrecision(18, 2);
    }
}
