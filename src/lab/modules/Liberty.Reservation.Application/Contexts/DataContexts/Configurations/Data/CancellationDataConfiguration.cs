using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class CancellationDataConfiguration : BaseDataEntityTypeConfiguration<CancellationData>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<CancellationData> builder
    )
    {
        builder.ToTable("CancellationData", DbConfiguration.DefaultSchema);

        builder
            .HasMany(c => c.CancellationCancellationData)
            .WithOne(c => c.CancellationData)
            .HasForeignKey(c => c.CancellationDataId);
    }
}
