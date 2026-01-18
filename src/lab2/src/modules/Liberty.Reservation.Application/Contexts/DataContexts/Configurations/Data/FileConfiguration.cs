using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using File = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.File;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class FileConfiguration : BaseDataEntityTypeConfiguration<File>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<File> builder
    )
    {
        builder.ToTable("file", DbConfiguration.DefaultSchema);

        builder
            .HasMany(c => c.FileCategories)
            .WithOne(c => c.File)
            .HasForeignKey(c => c.FileId);

        // 中間テーブルとの関係性登録
        builder
            .HasMany(c => c.FileRoomGroups)
            .WithOne(c => c.File)
            .HasForeignKey(c => c.FileId);

        // 中間テーブルとの関係性登録
        builder
            .HasMany(c => c.FilePlans)
            .WithOne(c => c.File)
            .HasForeignKey(c => c.FileId);

        builder
            .HasMany(c => c.FacilityFiles)
            .WithOne(c => c.File)
            .HasForeignKey(c => c.FileId);

        builder
            .HasMany(c => c.FilePlans)
            .WithOne(c => c.File)
            .HasForeignKey(c => c.FileId);

        builder
            .HasMany(c => c.FileOptionItems)
            .WithOne(c => c.File)
            .HasForeignKey(c => c.FileId);

        // 中間テーブルとの関係性登録
        builder
            .HasMany(c => c.FileRooms)
            .WithOne(c => c.File)
            .HasForeignKey(c => c.FileId);
    }
}
