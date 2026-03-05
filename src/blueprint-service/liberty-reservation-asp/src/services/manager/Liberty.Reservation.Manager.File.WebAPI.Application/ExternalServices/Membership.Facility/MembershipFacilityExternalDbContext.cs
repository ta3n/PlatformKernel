using Liberty.Reservation.Manager.File.WebAPI.Application.ExternalServices.Membership.Facility.Models;

namespace Liberty.Reservation.Manager.File.WebAPI.Application.ExternalServices.Membership.Facility;

public class MembershipFacilityExternalDbContext(
    DbContextOptions<MembershipFacilityExternalDbContext> options
) : DbContext(options)
{
    public DbSet<Models.Facility> Facilities { get; set; }
    public DbSet<Address> Addresses { get; set; }

    protected override void OnModelCreating(
        ModelBuilder modelBuilder
    )
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(MembershipFacilityExternalDbContext).Assembly
        );
    }
}
