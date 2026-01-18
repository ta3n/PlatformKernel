using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class PlanRoomGroupSiteDiscountDataConfiguration
    : BaseRelationEntityTypeConfiguration<PlanRoomGroupSiteDiscountData>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<PlanRoomGroupSiteDiscountData> builder
    )
    {
        builder.ToTable("plan_room_group_site_discount_data", DbConfiguration.DefaultSchema);

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.PlanId,
                    c.RoomGroupId,
                    c.SiteId,
                    c.DiscountDataId
                }
            );

        builder
            .HasOne(sc => sc.Plan)
            .WithMany(s => s.PlanRoomGroupSiteDiscountData)
            .HasForeignKey(sc => sc.PlanId);

        builder
            .HasOne(sc => sc.RoomGroup)
            .WithMany(s => s.PlanRoomGroupSiteDiscountData)
            .HasForeignKey(sc => sc.RoomGroupId);

        builder
            .HasOne(sc => sc.Site)
            .WithMany(s => s.PlanRoomGroupSiteDiscountData)
            .HasForeignKey(sc => sc.SiteId);

        builder
            .HasOne(sc => sc.DiscountData)
            .WithMany(s => s.PlanRoomGroupSiteDiscountData)
            .HasForeignKey(sc => sc.DiscountDataId);
    }
}
