using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class DiscountDataConfiguration : BaseDataEntityTypeConfiguration<DiscountData>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<DiscountData> builder
    )
    {
        builder.ToTable("discount_data", DbConfiguration.DefaultSchema);

        builder
            .HasMany(c => c.PlanRoomGroupSiteDiscountData)
            .WithOne(c => c.DiscountData)
            .HasForeignKey(c => c.DiscountDataId);
    }
}
