using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class FacilityAllergenConfiguration : BaseRelationEntityTypeConfiguration<FacilityAllergen>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<FacilityAllergen> builder
    )
    {
        builder.ToTable("FacilityAllergen", DbConfiguration.DefaultSchema);

        builder.Property(x => x.FacilityId).HasColumnName("FacilityID");
        builder.Property(x => x.AllergenId).HasColumnName("AllergenID");

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.FacilityId,
                    c.AllergenId
                }
            );

        builder
            .HasOne(sc => sc.Facility)
            .WithMany(s => s.FacilityAllergens)
            .HasForeignKey(sc => sc.FacilityId);

        builder
            .HasOne(sc => sc.Allergen)
            .WithMany(s => s.FacilityAllergens)
            .HasForeignKey(sc => sc.AllergenId);
    }
}
