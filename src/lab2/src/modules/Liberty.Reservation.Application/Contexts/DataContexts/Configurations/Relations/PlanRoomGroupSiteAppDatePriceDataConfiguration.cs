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
        builder.ToTable("plan_room_group_site_app_date_price_data", DbConfiguration.DefaultSchema);

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.PlanId,
                    c.RoomGroupId,
                    c.SiteId,
                    c.DateCalendar,
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
            .HasOne(sc => sc.PriceData)
            .WithMany(s => s.PlanRoomGroupSiteAppDatePriceData)
            .HasForeignKey(sc => sc.PriceDataId);
    }
}
