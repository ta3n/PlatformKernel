using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class CancellationDataConfiguration : BaseDataEntityTypeConfiguration<CancellationData>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<CancellationData> builder
    )
    {
        builder.ToTable("cancellation_data", DbConfiguration.DefaultSchema);

        builder.Property(e => e.Description)
            .HasColumnType("jsonb")
            .HasConversion(MultilingualTextConverter())
            .Metadata.SetValueComparer(typeof(MultilingualTextComparer));

        builder.Property(e => e.Description)
            .HasDefaultValueSql("'{}'::jsonb");

        builder
            .HasIndex(x => x.Description)
            .HasMethod("GIN");

        builder
            .HasMany(c => c.CancellationCancellationData)
            .WithOne(c => c.CancellationData)
            .HasForeignKey(c => c.CancellationDataId);
    }
}
