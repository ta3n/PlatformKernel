using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class FaxServiceConfiguration : BaseDataEntityTypeConfiguration<FaxService>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<FaxService> builder
    )
    {
        builder.ToTable("FaxService", DbConfiguration.DefaultSchema);

        builder
            .HasMany(c => c.FacilityFaxServices)
            .WithOne(c => c.FaxService)
            .HasForeignKey(c => c.FaxServiceId);
    }
}
