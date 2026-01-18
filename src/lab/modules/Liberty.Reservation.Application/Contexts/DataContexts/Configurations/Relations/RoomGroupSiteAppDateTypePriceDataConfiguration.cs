using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class RoomGroupSiteAppDateTypePriceDataConfiguration
    : BaseRelationEntityTypeConfiguration<RoomGroupSiteAppDateTypePriceData>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<RoomGroupSiteAppDateTypePriceData> builder
    )
    {
        builder.ToTable("RoomGroupSiteAppDateTypePriceData", DbConfiguration.DefaultSchema);

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    RoomGroupID = c.RoomGroupId,
                    SiteID = c.SiteId,
                    AppDateTypeID = c.AppDateTypeId,
                    PriceDataID = c.PriceDataId
                }
            );

        builder
            .HasOne(sc => sc.RoomGroup)
            .WithMany(s => s.RoomGroupSiteAppDateTypePriceData)
            .HasForeignKey(sc => sc.RoomGroupId);

        builder
            .HasOne(sc => sc.Site)
            .WithMany(s => s.RoomGroupSiteAppDateTypePriceData)
            .HasForeignKey(sc => sc.SiteId);

        builder
            .HasOne(sc => sc.AppDateType)
            .WithMany(s => s.RoomGroupSiteAppDateTypePriceData)
            .HasForeignKey(sc => sc.AppDateTypeId);

        builder
            .HasOne(sc => sc.PriceData)
            .WithMany(s => s.RoomGroupSiteAppDateTypePriceData)
            .HasForeignKey(sc => sc.PriceDataId);
    }
}
