using Liberty.Reservation.Application.Contexts.DataContexts;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liberty.Reservation.User.WebAPI.Application.ExternalServices.Membership.Facility.Models.Configurations;

public class FacilityConfiguration : IEntityTypeConfiguration<Facility>
{
    public void Configure(
        EntityTypeBuilder<Facility> builder
    )
    {
        builder.ToTable("facility", DbConfiguration.DefaultSchema);

        builder.Property(a => a.Id).ValueGeneratedNever();
        builder.Property(a => a.Code).IsRequired();
        builder.HasQueryFilter(p => p.State != FacilityStates.Deleted);
    }
}
