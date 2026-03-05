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
        builder.ToTable("plan_room_group_site_app_date_type_price_data", DbConfiguration.DefaultSchema);

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
