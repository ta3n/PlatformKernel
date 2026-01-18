using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class SitePointRateConfiguration : BaseRelationEntityTypeConfiguration<SitePointRate>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<SitePointRate> builder
    )
    {
        builder.ToTable("site_point_rate", DbConfiguration.DefaultSchema);

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.SiteId,
                    c.PointRateId
                }
            );

        builder
            .HasOne(sc => sc.Site)
            .WithMany(s => s.SitePointRates)
            .HasForeignKey(sc => sc.SiteId);

        builder
            .HasOne(sc => sc.PointRate)
            .WithMany(s => s.SitePointRates)
            .HasForeignKey(sc => sc.PointRateId);
    }
}
