using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class PlanRoomGroupSiteAppDateConfiguration : BaseRelationEntityTypeConfiguration<PlanRoomGroupSiteAppDate>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<PlanRoomGroupSiteAppDate> builder
    )
    {
        builder.ToTable("PlanRoomGroupSiteAppDate", DbConfiguration.DefaultSchema);

        builder.Property(x => x.PlanId).HasColumnName("PlanID");
        builder.Property(x => x.RoomGroupId).HasColumnName("RoomGroupID");
        builder.Property(x => x.SiteId).HasColumnName("SiteID");
        builder.Property(x => x.AppDateId).HasColumnName("AppDateID");

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.PlanId,
                    c.RoomGroupId,
                    c.SiteId,
                    c.AppDateId
                }
            );

        builder
            .HasOne(sc => sc.Plan)
            .WithMany(s => s.PlanRoomGroupSiteAppDates)
            .HasForeignKey(sc => sc.PlanId);

        builder
            .HasOne(sc => sc.RoomGroup)
            .WithMany(s => s.PlanRoomGroupSiteAppDates)
            .HasForeignKey(sc => sc.RoomGroupId);

        builder
            .HasOne(sc => sc.Site)
            .WithMany(s => s.PlanRoomGroupSiteAppDates)
            .HasForeignKey(sc => sc.SiteId);

        builder
            .HasOne(sc => sc.AppDate)
            .WithMany(s => s.PlanRoomGroupSiteAppDates)
            .HasForeignKey(sc => sc.AppDateId);
    }
}
