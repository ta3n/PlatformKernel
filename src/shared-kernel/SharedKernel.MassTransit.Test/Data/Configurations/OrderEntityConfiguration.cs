using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.MassTransit.Test.Domain;

namespace SharedKernel.MassTransit.Test.Data.Configurations;

public sealed class OrderEntityConfiguration : IEntityTypeConfiguration<OrderEntity>
{
    public void Configure(
        EntityTypeBuilder<OrderEntity> builder
    )
    {
        builder.ToTable("test_orders");

        builder.HasKey(order => order.Id);

        builder.Property(order => order.CustomerId)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(order => order.Amount)
            .HasPrecision(18, 2);

        builder.Property(order => order.Status)
            .HasMaxLength(64)
            .IsRequired();
    }
}
