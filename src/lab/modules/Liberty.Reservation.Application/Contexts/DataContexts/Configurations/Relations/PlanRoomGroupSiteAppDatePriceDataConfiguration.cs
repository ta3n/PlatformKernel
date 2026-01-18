using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class PlanRoomGroupSiteAppDatePriceDataConfiguration
    : BaseRelationEntityTypeConfiguration<PlanRoomGroupSiteAppDatePriceData>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<PlanRoomGroupSiteAppDatePriceData> builder
    )
    {
        builder.ToTable("PlanRoomGroupSiteAppDatePriceData", DbConfiguration.DefaultSchema);

        builder.Property(x => x.PlanId).HasColumnName("PlanID");
        builder.Property(x => x.RoomGroupId).HasColumnName("RoomGroupID");
        builder.Property(x => x.SiteId).HasColumnName("SiteID");
        builder.Property(x => x.AppDateId).HasColumnName("AppDateID");
        builder.Property(x => x.PriceDataId).HasColumnName("PriceDataID");

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.PlanId,
                    c.RoomGroupId,
                    c.SiteId,
                    c.AppDateId,
                    c.PriceDataId
                }
            );

        builder
            .HasOne(sc => sc.Plan)
            .WithMany(s => s.PlanRoomGroupSiteAppDatePriceData)
            .HasForeignKey(sc => sc.PlanId);

        builder
            .HasOne(sc => sc.RoomGroup)
            .WithMany(s => s.PlanRoomGroupSiteAppDatePriceData)
            .HasForeignKey(sc => sc.RoomGroupId);

        builder
            .HasOne(sc => sc.AppDate)
            .WithMany(s => s.PlanRoomGroupSiteAppDatePriceDatas)
            .HasForeignKey(sc => sc.AppDateId);

        builder
            .HasOne(sc => sc.PriceData)
            .WithMany(s => s.PlanRoomGroupSiteAppDatePriceData)
            .HasForeignKey(sc => sc.PriceDataId);
    }
}
