using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class AllergenConfiguration : BaseDataEntityTypeConfiguration<Allergen>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<Allergen> builder
    )
    {
        builder.ToTable("allergen", DbConfiguration.DefaultSchema);

        builder
            .HasMany(c => c.FacilityAllergens)
            .WithOne(c => c.Allergen)
            .HasForeignKey(c => c.AllergenId);
    }
}
