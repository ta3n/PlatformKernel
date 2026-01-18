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
        builder.ToTable("RoomGroupSiteAppDatePriceData", DbConfiguration.DefaultSchema);

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    RoomGroupID = c.RoomGroupId,
                    SiteID = c.SiteId,
                    AppDateID = c.AppDateId,
                    PriceDataID = c.PriceDataId
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
