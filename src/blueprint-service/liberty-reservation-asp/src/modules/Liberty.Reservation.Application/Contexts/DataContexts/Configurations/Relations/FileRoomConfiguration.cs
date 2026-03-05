using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class FileRoomConfiguration : BaseRelationEntityTypeConfiguration<FileRoom>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<FileRoom> builder
    )
    {
        builder.ToTable("file_room", DbConfiguration.DefaultSchema);

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.FileId,
                    c.RoomId
                }
            );

        builder
            .HasOne(sc => sc.File)
            .WithMany(s => s.FileRooms)
            .HasForeignKey(sc => sc.FileId);

        builder
            .HasOne(sc => sc.Room)
            .WithMany(s => s.FileRooms)
            .HasForeignKey(sc => sc.RoomId);
    }
}
