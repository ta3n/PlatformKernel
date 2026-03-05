using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class FacilityOptionItemConfiguration : BaseRelationEntityTypeConfiguration<FacilityOptionItem>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<FacilityOptionItem> builder
    )
    {
        builder.ToTable("facility_option_item", DbConfiguration.DefaultSchema);

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.FacilityId,
                    c.OptionItemId
                }
            );

        builder
            .HasOne(sc => sc.Facility)
            .WithMany(s => s.FacilityOptionItems)
            .HasForeignKey(sc => sc.FacilityId);

        builder
            .HasOne(sc => sc.OptionItem)
            .WithMany(s => s.FacilityOptionItems)
            .HasForeignKey(sc => sc.OptionItemId);
    }
}
