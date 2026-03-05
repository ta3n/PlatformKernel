using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class AdjustmentResultConfiguration : BaseDataEntityTypeConfiguration<AdjustmentResult>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<AdjustmentResult> builder
    )
    {
        builder.ToTable("adjustment_result", DbConfiguration.DefaultSchema);

        builder.Property(x => x.RoomAdjustmentStatusId).IsRequired();

        builder.HasOne(x => x.RoomAdjustmentStatus)
            .WithMany(x => x.AdjustmentResults)
            .HasForeignKey(x => x.RoomAdjustmentStatusId);
    }
}
