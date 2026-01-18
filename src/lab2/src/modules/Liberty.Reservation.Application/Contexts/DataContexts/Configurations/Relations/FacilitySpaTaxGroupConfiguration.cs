using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class FacilitySpaTaxGroupConfiguration : BaseRelationEntityTypeConfiguration<FacilitySpaTaxGroup>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<FacilitySpaTaxGroup> builder
    )
    {
        builder.ToTable("facility_spa_tax_group", DbConfiguration.DefaultSchema);

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.FacilityId,
                    c.SpaTaxGroupId
                }
            );

        builder
            .HasOne(sc => sc.Facility)
            .WithMany(s => s.FacilitySpaTaxGroups)
            .HasForeignKey(sc => sc.FacilityId);

        builder
            .HasOne(sc => sc.SpaTaxGroup)
            .WithMany(s => s.FacilitySpaTaxGroups)
            .HasForeignKey(sc => sc.SpaTaxGroupId);
    }
}
