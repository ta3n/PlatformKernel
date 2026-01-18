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
        builder.ToTable("FileOptionItem", DbConfiguration.DefaultSchema);

        builder.Property(x => x.FileId).HasColumnName("FileID");
        builder.Property(x => x.OptionItemId).HasColumnName("OptionItemID");

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.FileId,
                    c.OptionItemId
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
