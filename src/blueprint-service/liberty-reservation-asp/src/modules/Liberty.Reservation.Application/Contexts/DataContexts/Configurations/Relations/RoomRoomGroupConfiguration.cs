using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class RoomRoomGroupConfiguration : BaseRelationEntityTypeConfiguration<RoomRoomGroup>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<RoomRoomGroup> builder
    )
    {
        builder.ToTable("room_room_group", DbConfiguration.DefaultSchema);

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.RoomId,
                    c.RoomGroupId
                }
            );

        builder
            .HasOne(sc => sc.Room)
            .WithMany(s => s.RoomRoomGroups)
            .HasForeignKey(sc => sc.RoomId);

        builder
            .HasOne(sc => sc.RoomGroup)
            .WithMany(s => s.RoomRoomGroups)
            .HasForeignKey(sc => sc.RoomGroupId);
    }
}
