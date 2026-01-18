using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class PlanCategoryConfiguration : BaseRelationEntityTypeConfiguration<PlanCategory>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<PlanCategory> builder
    )
    {
        builder.ToTable("PlanCategory", DbConfiguration.DefaultSchema);

        builder.Property(x => x.PlanId).HasColumnName("PlanID");
        builder.Property(x => x.CategoryId).HasColumnName("CategoryID");

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.PlanId,
                    c.CategoryId
                }
            );

        builder
            .HasOne(sc => sc.Category)
            .WithMany(s => s.PlanCategories)
            .HasForeignKey(sc => sc.CategoryId);

        builder
            .HasOne(sc => sc.Plan)
            .WithMany(s => s.PlanCategories)
            .HasForeignKey(sc => sc.PlanId);
    }
}
