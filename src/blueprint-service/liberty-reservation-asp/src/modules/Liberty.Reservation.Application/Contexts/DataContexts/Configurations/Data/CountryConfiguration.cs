using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class CountryConfiguration : BaseDataEntityTypeConfiguration<Country>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<Country> builder
    )
    {
        builder.ToTable("country", DbConfiguration.DefaultSchema);
    }
}
