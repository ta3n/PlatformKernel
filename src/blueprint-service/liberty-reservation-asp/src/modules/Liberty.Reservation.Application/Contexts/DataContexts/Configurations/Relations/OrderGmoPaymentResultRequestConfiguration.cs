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
        builder.ToTable("order_gmo_payment_result_request", DbConfiguration.DefaultSchema);

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.OrderId,
                    c.GmoPaymentResultRequestId
                }
            );

        builder
            .HasOne(sc => sc.Order)
            .WithMany(s => s.OrderGmoPaymentResultRequests)
            .HasForeignKey(sc => sc.OrderId);

        builder
            .HasOne(sc => sc.GmoPaymentResultRequest)
            .WithMany(s => s.OrderGmoPaymentResultRequests)
            .HasForeignKey(sc => sc.GmoPaymentResultRequestId);
    }
}
