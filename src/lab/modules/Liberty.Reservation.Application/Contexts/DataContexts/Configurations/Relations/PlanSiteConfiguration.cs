using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class PlanSiteConfiguration : BaseRelationEntityTypeConfiguration<PlanSite>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<PlanSite> builder
    )
    {
        builder.ToTable("PlanSite", DbConfiguration.DefaultSchema);

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    PlanID = c.PlanId,
                    SiteID = c.SiteId
                }
            );

        builder
            .HasOne(sc => sc.Plan)
            .WithMany(s => s.PlanSites)
            .HasForeignKey(sc => sc.PlanId);

        builder
            .HasOne(sc => sc.Site)
            .WithMany(s => s.PlanSites)
            .HasForeignKey(sc => sc.SiteId);
    }
}
