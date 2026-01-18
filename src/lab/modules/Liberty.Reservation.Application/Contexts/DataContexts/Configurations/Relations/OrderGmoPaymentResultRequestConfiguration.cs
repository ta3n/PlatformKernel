using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class OrderGmoPaymentResultRequestConfiguration
    : BaseRelationEntityTypeConfiguration<OrderGmoPaymentResultRequest>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<OrderGmoPaymentResultRequest> builder
    )
    {
        builder.ToTable("OrderGmoPaymentResultRequest", DbConfiguration.DefaultSchema);

        builder.Property(x => x.OrderId).HasColumnName("OrderID");
        builder.Property(x => x.GmoPaymentResultRequestId).HasColumnName("GMOPaymentResultRequestID");

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    OrderID = c.OrderId,
                    GMOPaymentResultRequestID = c.GmoPaymentResultRequestId
                }
            );

        builder
            .HasOne(sc => sc.Order)
            .WithMany(s => s.OrderGMOPaymentResultRequests)
            .HasForeignKey(sc => sc.OrderId);

        builder
            .HasOne(sc => sc.GmoPaymentResultRequest)
            .WithMany(s => s.OrderGmoPaymentResultRequests)
            .HasForeignKey(sc => sc.GmoPaymentResultRequestId);
    }
}
