using Liberty.Reservation.Application.Contexts.DataContexts.Configurations;
using Liberty.Reservation.Employee.Application.Contexts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.Employee.Application.Contexts.Configurations;

public class AddressConfiguration : BaseDataEntityTypeConfiguration<Address>
{
    protected override void EntityConfigure(
        EntityTypeBuilder<Address> builder
    )
    {
        builder.ToTable("Address");
    }
}
