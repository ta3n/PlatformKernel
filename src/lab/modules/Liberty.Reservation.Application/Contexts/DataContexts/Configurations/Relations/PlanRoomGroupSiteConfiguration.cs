using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class PlanRoomGroupSiteConfiguration : BaseRelationEntityTypeConfiguration<PlanRoomGroupSite>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<PlanRoomGroupSite> builder
    )
    {
        builder.ToTable("PlanRoomGroupSite", DbConfiguration.DefaultSchema);

        builder.Property(x => x.PlanId).HasColumnName("PlanID");
        builder.Property(x => x.RoomGroupId).HasColumnName("RoomGroupID");
        builder.Property(x => x.SiteId).HasColumnName("SiteID");

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.PlanId,
                    c.RoomGroupId,
                    c.SiteId
                }
            );

        builder
            .HasOne(sc => sc.Plan)
            .WithMany(s => s.PlanRoomGroupSites)
            .HasForeignKey(sc => sc.PlanId);

        builder
            .HasOne(sc => sc.RoomGroup)
            .WithMany(s => s.PlanRoomGroupSites)
            .HasForeignKey(sc => sc.RoomGroupId);

        builder
            .HasOne(sc => sc.Site)
            .WithMany(s => s.PlanRoomGroupSites)
            .HasForeignKey(sc => sc.SiteId);
    }
}
