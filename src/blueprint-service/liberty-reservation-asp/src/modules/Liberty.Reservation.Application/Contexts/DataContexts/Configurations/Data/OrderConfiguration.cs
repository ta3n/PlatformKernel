using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class OrderConfiguration : BaseDataEntityTypeConfiguration<Order>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<Order> builder
    )
    {
        builder.ToTable("order", DbConfiguration.DefaultSchema);

        builder
            .Property(u => u.ApiIssueCode)
            .IsRequired();

        builder
            .HasIndex(u => u.ApiIssueCode)
            .IsUnique();

        // 中間テーブルとの関係性登録
        builder
            .HasMany(c => c.OrderReservations)
            .WithOne(c => c.Order)
            .HasForeignKey(c => c.OrderId);

        // 中間テーブルとの関係性登録
        builder
            .HasMany(c => c.OrderGmoPaymentResultRequests)
            .WithOne(c => c.Order)
            .HasForeignKey(c => c.OrderId);
    }
}
