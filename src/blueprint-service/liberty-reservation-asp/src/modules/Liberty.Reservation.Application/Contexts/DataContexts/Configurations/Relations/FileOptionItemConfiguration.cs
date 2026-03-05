using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class FileOptionItemConfiguration : BaseRelationEntityTypeConfiguration<FileOptionItem>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<FileOptionItem> builder
    )
    {
        builder.ToTable("file_option_item", DbConfiguration.DefaultSchema);

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.FileId,
                    c.OptionItemId,
                    c.Index
                }
            );

        builder
            .HasOne(sc => sc.File)
            .WithMany(s => s.FileOptionItems)
            .HasForeignKey(sc => sc.FileId);

        builder
            .HasOne(sc => sc.OptionItem)
            .WithMany(s => s.FileOptionItems)
            .HasForeignKey(sc => sc.OptionItemId);
    }
}
