using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class FacilitySiteConfiguration : BaseRelationEntityTypeConfiguration<FacilitySite>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<FacilitySite> builder
    )
    {
        builder.ToTable("FacilitySite", DbConfiguration.DefaultSchema);

        builder.Property(x => x.FacilityId).HasColumnName("FacilityID");
        builder.Property(x => x.SiteId).HasColumnName("SiteID");

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.FacilityId,
                    c.SiteId
                }
            );

        builder
            .HasOne(sc => sc.Facility)
            .WithMany(s => s.FacilitySites)
            .HasForeignKey(sc => sc.FacilityId);

        builder
            .HasOne(sc => sc.Site)
            .WithMany(s => s.FacilitySites)
            .HasForeignKey(sc => sc.SiteId);
    }
}
