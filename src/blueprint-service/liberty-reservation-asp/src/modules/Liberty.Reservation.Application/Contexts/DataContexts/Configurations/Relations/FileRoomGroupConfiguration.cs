using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class FileRoomGroupConfiguration : BaseRelationEntityTypeConfiguration<FileRoomGroup>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<FileRoomGroup> builder
    )
    {
        builder.ToTable("file_room_group", DbConfiguration.DefaultSchema);

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.FileId,
                    c.RoomGroupId,
                    c.Index
                }
            );

        builder
            .HasOne(sc => sc.File)
            .WithMany(s => s.FileRoomGroups)
            .HasForeignKey(sc => sc.FileId);

        builder
            .HasOne(sc => sc.RoomGroup)
            .WithMany(s => s.FileRoomGroups)
            .HasForeignKey(sc => sc.RoomGroupId);
    }
}
