using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class SpaTaxDataConfiguration : BaseDataEntityTypeConfiguration<SpaTaxData>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<SpaTaxData> builder
    )
    {
        builder.ToTable("SpaTaxData", DbConfiguration.DefaultSchema);

        builder
            .HasMany(c => c.SpaTaxGroupSpaTaxData)
            .WithOne(c => c.SpaTaxData)
            .HasForeignKey(c => c.SpaTaxDataId);

        builder
            .HasMany(c => c.PersonAgeTypeSpaTaxData)
            .WithOne(c => c.SpaTaxData)
            .HasForeignKey(c => c.SpaTaxDataId);
    }
}
