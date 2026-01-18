using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class FileCategoryConfiguration : BaseRelationEntityTypeConfiguration<FileCategory>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<FileCategory> builder
    )
    {
        builder.ToTable("file_category", DbConfiguration.DefaultSchema);

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.FileId,
                    c.CategoryId
                }
            );

        builder
            .HasOne(sc => sc.File)
            .WithMany(s => s.FileCategories)
            .HasForeignKey(sc => sc.FileId);

        builder
            .HasOne(sc => sc.Category)
            .WithMany(s => s.FileCategories)
            .HasForeignKey(sc => sc.CategoryId);
    }
}
