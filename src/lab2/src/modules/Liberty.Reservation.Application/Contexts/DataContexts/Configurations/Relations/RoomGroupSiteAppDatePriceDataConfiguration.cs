using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class RoomGroupSiteAppDatePriceDataConfiguration
    : BaseRelationEntityTypeConfiguration<RoomGroupSiteAppDatePriceData>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<RoomGroupSiteAppDatePriceData> builder
    )
    {
        builder.ToTable("room_group_site_app_date_price_data", DbConfiguration.DefaultSchema);

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    c.RoomGroupId,
                    c.SiteId,
                    c.AppDateId,
                    c.PriceDataId
                }
            );

        builder
            .HasOne(sc => sc.RoomGroup)
            .WithMany(s => s.RoomGroupSiteAppDatePriceData)
            .HasForeignKey(sc => sc.RoomGroupId);

        builder
            .HasOne(sc => sc.Site)
            .WithMany(s => s.RoomGroupSiteAppDatePriceData)
            .HasForeignKey(sc => sc.SiteId);

        builder
            .HasOne(sc => sc.AppDate)
            .WithMany(s => s.RoomGroupSiteAppDatePriceDatas)
            .HasForeignKey(sc => sc.AppDateId);

        builder
            .HasOne(sc => sc.PriceData)
            .WithMany(s => s.RoomGroupSiteAppDatePriceData)
            .HasForeignKey(sc => sc.PriceDataId);
    }
}
