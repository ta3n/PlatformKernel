using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class PlanRoomGroupCancellationConfiguration : BaseRelationEntityTypeConfiguration<PlanRoomGroupCancellation>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<PlanRoomGroupCancellation> builder
    )
    {
        builder.ToTable("PlanRoomGroupCancellation", DbConfiguration.DefaultSchema);

        builder.Property(x => x.PlanId).HasColumnName("PlanID");
        builder.Property(x => x.RoomGroupId).HasColumnName("RoomGroupID");
        builder.Property(x => x.CancellationId).HasColumnName("CancellationID");

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.PlanId,
                    c.RoomGroupId,
                    c.CancellationId
                }
            );

        builder
            .HasOne(sc => sc.Plan)
            .WithMany(s => s.PlanRoomGroupCancellations)
            .HasForeignKey(sc => sc.PlanId);

        builder
            .HasOne(sc => sc.RoomGroup)
            .WithMany(s => s.PlanRoomGroupCancellations)
            .HasForeignKey(sc => sc.RoomGroupId);

        builder
            .HasOne(sc => sc.Cancellation)
            .WithMany(s => s.PlanRoomGroupCancellations)
            .HasForeignKey(sc => sc.CancellationId);
    }
}
