using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class FacilityRoomGroupConfiguration : BaseRelationEntityTypeConfiguration<FacilityRoomGroup>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<FacilityRoomGroup> builder
    )
    {
        builder.ToTable("FacilityRoomGroup", DbConfiguration.DefaultSchema);

        builder.Property(x => x.FacilityId).HasColumnName("FacilityID");
        builder.Property(x => x.RoomGroupId).HasColumnName("RoomGroupID");

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.FacilityId,
                    c.RoomGroupId
                }
            );

        builder
            .HasOne(sc => sc.Facility)
            .WithMany(s => s.FacilityRoomGroups)
            .HasForeignKey(sc => sc.FacilityId);

        builder
            .HasOne(sc => sc.RoomGroup)
            .WithMany(s => s.FacilityRoomGroups)
            .HasForeignKey(sc => sc.RoomGroupId);
    }
}
