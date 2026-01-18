using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class RoomGroupSiteConfiguration : BaseRelationEntityTypeConfiguration<RoomGroupSite>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<RoomGroupSite> builder
    )
    {
        builder.ToTable("RoomGroupSite", DbConfiguration.DefaultSchema);

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    RoomGroupID = c.RoomGroupId,
                    SiteID = c.SiteId
                }
            );

        builder
            .HasOne(sc => sc.RoomGroup)
            .WithMany(s => s.RoomGroupSites)
            .HasForeignKey(sc => sc.RoomGroupId);

        builder
            .HasOne(sc => sc.Site)
            .WithMany(s => s.RoomGroupSites)
            .HasForeignKey(sc => sc.SiteId);
    }
}
