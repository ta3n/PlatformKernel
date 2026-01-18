using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class RoomAdjustmentStatusConfiguration : BaseDataEntityTypeConfiguration<RoomAdjustmentStatus>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<RoomAdjustmentStatus> builder
    )
    {
        builder.ToTable("room_adjustment_status", DbConfiguration.DefaultSchema);

        builder.HasMany(x => x.AdjustmentResults)
            .WithOne(x => x.RoomAdjustmentStatus)
            .HasForeignKey(x => x.RoomAdjustmentStatusId);
    }
}
