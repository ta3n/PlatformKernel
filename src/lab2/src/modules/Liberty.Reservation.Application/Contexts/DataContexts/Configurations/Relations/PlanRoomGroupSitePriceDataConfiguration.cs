using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class PlanRoomGroupSitePriceDataConfiguration
    : BaseRelationEntityTypeConfiguration<PlanRoomGroupSitePriceData>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<PlanRoomGroupSitePriceData> builder
    )
    {
        builder.ToTable("plan_room_group_site_price_data", DbConfiguration.DefaultSchema);

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.PlanId,
                    c.RoomGroupId,
                    c.SiteId,
                    c.PersonMin,
                    c.PersonMax
                }
            );

        builder
            .HasOne(sc => sc.Plan)
            .WithMany(s => s.PlanRoomGroupSitePriceData)
            .HasForeignKey(sc => sc.PlanId);

        builder
            .HasOne(sc => sc.RoomGroup)
            .WithMany(s => s.PlanRoomGroupSitePriceData)
            .HasForeignKey(sc => sc.RoomGroupId);

        builder
            .HasOne(sc => sc.Site)
            .WithMany(s => s.PlanRoomGroupSitePriceData)
            .HasForeignKey(sc => sc.SiteId);
    }
}
