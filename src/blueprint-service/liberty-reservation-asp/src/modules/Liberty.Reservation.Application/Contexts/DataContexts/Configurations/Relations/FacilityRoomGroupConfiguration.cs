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
        builder.ToTable("facility_room_group", DbConfiguration.DefaultSchema);

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
