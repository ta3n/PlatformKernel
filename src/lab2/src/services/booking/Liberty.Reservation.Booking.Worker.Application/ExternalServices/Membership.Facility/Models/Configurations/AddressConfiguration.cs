using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Booking.Worker.Application.ExternalServices.Membership.Facility.Models.Configurations;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(
        EntityTypeBuilder<Address> builder
    )
    {
        builder.ToTable("Addresss");
    }
}
