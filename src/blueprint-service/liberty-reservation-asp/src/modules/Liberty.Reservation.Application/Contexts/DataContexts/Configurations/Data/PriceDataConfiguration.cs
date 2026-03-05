using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class PriceDataConfiguration : BaseDataEntityTypeConfiguration<PriceData>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<PriceData> builder
    )
    {
        builder.ToTable("price_data", DbConfiguration.DefaultSchema);

        builder
            .HasMany(c => c.RoomGroupAppDateTypePriceDatas)
            .WithOne(c => c.PriceData)
            .HasForeignKey(c => c.PriceDataId);

        builder
            .HasMany(c => c.RoomGroupSiteAppDatePriceData)
            .WithOne(c => c.PriceData)
            .HasForeignKey(c => c.PriceDataId);

        builder
            .HasMany(c => c.RoomGroupSiteAppDateTypePriceData)
            .WithOne(c => c.PriceData)
            .HasForeignKey(c => c.PriceDataId);

        builder
            .HasMany(c => c.PlanRoomGroupSiteAppDatePriceData)
            .WithOne(c => c.PriceData)
            .HasForeignKey(c => c.PriceDataId);

        builder
            .HasMany(c => c.PlanRoomGroupSiteAppDateTypePriceData)
            .WithOne(c => c.PriceData)
            .HasForeignKey(c => c.PriceDataId);
    }
}
