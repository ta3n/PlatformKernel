using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class GmoPaymentResultRequestConfiguration : BaseDataEntityTypeConfiguration<GmoPaymentResultRequest>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<GmoPaymentResultRequest> builder
    )
    {
        builder.ToTable("gmo_payment_result_request", DbConfiguration.DefaultSchema);

        builder
            .HasMany(c => c.OrderGmoPaymentResultRequests)
            .WithOne(c => c.GmoPaymentResultRequest)
            .HasForeignKey(c => c.GmoPaymentResultRequestId);
    }
}
