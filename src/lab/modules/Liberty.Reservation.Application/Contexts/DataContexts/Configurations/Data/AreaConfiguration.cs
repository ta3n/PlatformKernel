using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class AreaConfiguration : BaseDataEntityTypeConfiguration<Area>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<Area> builder
    )
    {
        builder.ToTable("Area", DbConfiguration.DefaultSchema);
    }
}
