using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class OptionItemCategoryConfiguration : BaseRelationEntityTypeConfiguration<OptionItemCategory>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<OptionItemCategory> builder
    )
    {
        builder.ToTable("option_item_category", DbConfiguration.DefaultSchema);

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.OptionItemId,
                    c.CategoryId
                }
            );

        builder
            .HasOne(sc => sc.OptionItem)
            .WithMany(s => s.OptionItemCategories)
            .HasForeignKey(sc => sc.OptionItemId);

        builder
            .HasOne(sc => sc.Category)
            .WithMany(s => s.OptionItemCategories)
            .HasForeignKey(sc => sc.CategoryId);
    }
}
