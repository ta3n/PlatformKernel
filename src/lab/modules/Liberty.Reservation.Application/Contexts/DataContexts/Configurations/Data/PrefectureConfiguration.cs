using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class PrefectureConfiguration : BaseDataEntityTypeConfiguration<Prefecture>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<Prefecture> builder
    )
    {
        builder.ToTable("Prefecture", DbConfiguration.DefaultSchema);
    }
}
