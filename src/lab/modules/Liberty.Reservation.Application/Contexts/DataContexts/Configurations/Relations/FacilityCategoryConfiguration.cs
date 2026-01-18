using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class FacilityCategoryConfiguration : BaseRelationEntityTypeConfiguration<FacilityCategory>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<FacilityCategory> builder
    )
    {
        builder.ToTable("FacilityCategory", DbConfiguration.DefaultSchema);

        builder.Property(x => x.FacilityId).HasColumnName("FacilityID");
        builder.Property(x => x.CategoryId).HasColumnName("CategoryID");

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.FacilityId,
                    c.CategoryId
                }
            );

        builder
            .HasOne(sc => sc.Facility)
            .WithMany(s => s.FacilityCategories)
            .HasForeignKey(sc => sc.FacilityId);

        builder
            .HasOne(sc => sc.Category)
            .WithMany(s => s.FacilityCategoryies)
            .HasForeignKey(sc => sc.CategoryId);
    }
}
