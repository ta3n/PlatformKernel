using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class PlanRoomGroupSiteAppDateTypePriceDataConfiguration
    : BaseRelationEntityTypeConfiguration<PlanRoomGroupSiteAppDateTypePriceData>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<PlanRoomGroupSiteAppDateTypePriceData> builder
    )
    {
        builder.ToTable("PlanRoomGroupSiteAppDateTypePriceData", DbConfiguration.DefaultSchema);

        builder.Property(x => x.PlanId).HasColumnName("PlanID");
        builder.Property(x => x.RoomGroupId).HasColumnName("RoomGroupID");
        builder.Property(x => x.SiteId).HasColumnName("SiteID");
        builder.Property(x => x.AppDateTypeId).HasColumnName("AppDateTypeID");
        builder.Property(x => x.PriceDataId).HasColumnName("PriceDataID");

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.PlanId,
                    c.RoomGroupId,
                    c.SiteId,
                    c.AppDateTypeId,
                    c.PriceDataId
                }
            );

        builder
            .HasOne(sc => sc.Plan)
            .WithMany(s => s.PlanRoomGroupSiteAppDateTypePriceData)
            .HasForeignKey(sc => sc.PlanId);

        builder
            .HasOne(sc => sc.RoomGroup)
            .WithMany(s => s.PlanRoomGroupSiteAppDateTypePriceData)
            .HasForeignKey(sc => sc.RoomGroupId);

        builder
            .HasOne(sc => sc.Site)
            .WithMany(s => s.PlanRoomGroupSiteAppDateTypePriceData)
            .HasForeignKey(sc => sc.SiteId);

        builder
            .HasOne(sc => sc.AppDateType)
            .WithMany(s => s.PlanRoomGroupSiteAppDateTypePriceData)
            .HasForeignKey(sc => sc.AppDateTypeId);
        builder
            .HasOne(sc => sc.PriceData)
            .WithMany(s => s.PlanRoomGroupSiteAppDateTypePriceData)
            .HasForeignKey(sc => sc.PriceDataId);
    }
}
