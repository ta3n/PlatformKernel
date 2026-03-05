using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Manager.WebAPI.Application.ExternalServices.Membership.Facility.Models.Configurations;

public class KeyValueConfiguration : IEntityTypeConfiguration<KeyValue>
{
    public void Configure(
        EntityTypeBuilder<KeyValue> builder
    )
    {
        builder.ToTable("KeyValues", "meta");

        builder.Property(a => a.Id).ValueGeneratedNever();
        builder.Property(a => a.Code).IsRequired();
    }
}
