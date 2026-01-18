using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Relations;

public class RoomGroupAppDateTypePriceDataConfiguration
    : BaseRelationEntityTypeConfiguration<RoomGroupAppDateTypePriceData>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<RoomGroupAppDateTypePriceData> builder
    )
    {
        builder.ToTable("RoomGroupAppDateTypePriceData", DbConfiguration.DefaultSchema);

        // 中間テーブル登録
        builder
            .HasKey(
                c => new
                {
                    RoomGroupID = c.RoomGroupId,
                    AppDateTypeID = c.AppDateTypeId,
                    PriceDataID = c.PriceDataId
                }
            );

        builder
            .HasOne(sc => sc.RoomGroup)
            .WithMany(s => s.RoomGroupAppDateTypePriceData)
            .HasForeignKey(sc => sc.RoomGroupId);

        builder
            .HasOne(sc => sc.AppDateType)
            .WithMany(s => s.RoomGroupAppDateTypePriceData)
            .HasForeignKey(sc => sc.AppDateTypeId);

        builder
            .HasOne(sc => sc.PriceData)
            .WithMany(s => s.RoomGroupAppDateTypePriceDatas)
            .HasForeignKey(sc => sc.PriceDataId);
    }
}
