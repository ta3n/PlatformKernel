using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class PlanRoomGroupConfiguration : BaseRelationEntityTypeConfiguration<PlanRoomGroup>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<PlanRoomGroup> builder
    )
    {
        builder.ToTable("PlanRoomGroup", DbConfiguration.DefaultSchema);

        builder.Property(x => x.PlanId).HasColumnName("PlanID");
        builder.Property(x => x.RoomGroupId).HasColumnName("RoomGroupID");

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.PlanId,
                    c.RoomGroupId
                }
            );

        builder
            .HasOne(sc => sc.RoomGroup)
            .WithMany(s => s.PlanRoomGroups)
            .HasForeignKey(sc => sc.RoomGroupId);

        builder
            .HasOne(sc => sc.Plan)
            .WithMany(s => s.PlanRoomGroups)
            .HasForeignKey(sc => sc.PlanId);
    }
}
