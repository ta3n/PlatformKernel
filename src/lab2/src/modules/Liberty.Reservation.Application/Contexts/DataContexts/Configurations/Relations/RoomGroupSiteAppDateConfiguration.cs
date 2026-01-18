using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class RoomGroupSiteAppDateConfiguration : BaseRelationEntityTypeConfiguration<RoomGroupSiteAppDate>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<RoomGroupSiteAppDate> builder
    )
    {
        builder.ToTable("room_group_site_app_date", DbConfiguration.DefaultSchema);

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.RoomGroupId,
                    c.SiteId,
                    c.AppDateId
                }
            );

        builder
            .HasOne(sc => sc.RoomGroup)
            .WithMany(s => s.RoomGroupSiteAppDates)
            .HasForeignKey(sc => sc.RoomGroupId);

        builder
            .HasOne(sc => sc.Site)
            .WithMany(s => s.RoomGroupSiteAppDates)
            .HasForeignKey(sc => sc.SiteId);

        builder
            .HasOne(sc => sc.AppDate)
            .WithMany(s => s.RoomGroupSiteAppDates)
            .HasForeignKey(sc => sc.AppDateId);
    }
}
