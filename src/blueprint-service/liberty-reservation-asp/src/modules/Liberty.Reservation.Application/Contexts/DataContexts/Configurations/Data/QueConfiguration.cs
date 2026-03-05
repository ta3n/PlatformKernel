using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class QueConfiguration : BaseDataEntityTypeConfiguration<Que>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<Que> builder
    )
    {
        builder.ToTable("que", DbConfiguration.DefaultSchema);
    }
}
