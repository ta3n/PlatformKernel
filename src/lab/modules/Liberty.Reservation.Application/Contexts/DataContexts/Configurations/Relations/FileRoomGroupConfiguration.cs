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
        builder.ToTable("FileRoomGroup", DbConfiguration.DefaultSchema);

        builder.Property(x => x.FileId).HasColumnName("FileID");
        builder.Property(x => x.RoomGroupId).HasColumnName("RoomGroupID");

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.FileId,
                    c.RoomGroupId
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
