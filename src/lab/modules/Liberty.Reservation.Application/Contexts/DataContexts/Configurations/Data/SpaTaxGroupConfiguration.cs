using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class SpaTaxGroupConfiguration : BaseDataEntityTypeConfiguration<SpaTaxGroup>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<SpaTaxGroup> builder
    )
    {
        builder.ToTable("SpaTaxGroup", DbConfiguration.DefaultSchema);

        builder
            .HasMany(c => c.FacilitySpaTaxGroups)
            .WithOne(c => c.SpaTaxGroup)
            .HasForeignKey(c => c.SpaTaxGroupId);

        builder
            .HasMany(c => c.SpaTaxGroupSpaTaxData)
            .WithOne(c => c.SpaTaxGroup)
            .HasForeignKey(c => c.SpaTaxGroupId);
    }
}
