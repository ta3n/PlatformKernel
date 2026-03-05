using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class PointRateConfiguration : BaseDataEntityTypeConfiguration<PointRate>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<PointRate> builder
    )
    {
        builder.ToTable("point_rate", DbConfiguration.DefaultSchema);

        builder
            .HasMany(c => c.SitePointRates)
            .WithOne(c => c.PointRate)
            .HasForeignKey(c => c.PointRateId);
    }
}
