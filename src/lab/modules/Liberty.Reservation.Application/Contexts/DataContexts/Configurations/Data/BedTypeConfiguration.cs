using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Configurations.Data;

public class BedTypeConfiguration : BaseDataEntityTypeConfiguration<BedType>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<BedType> builder
    )
    {
        builder.ToTable("BedType", DbConfiguration.DefaultSchema);

        builder
            .HasMany(c => c.RoomGroupBedType)
            .WithOne(c => c.BedType)
            .HasForeignKey(c => c.BedTypeId);
    }
}
