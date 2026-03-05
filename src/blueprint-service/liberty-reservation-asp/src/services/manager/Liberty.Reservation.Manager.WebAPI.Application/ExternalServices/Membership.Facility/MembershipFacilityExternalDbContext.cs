namespace Liberty.Reservation.Manager.WebAPI.Application.ExternalServices.Membership.Facility;

public class MembershipFacilityExternalDbContext(
    DbContextOptions<MembershipFacilityExternalDbContext> options
) : DbContext(options)
{
    public DbSet<Models.Facility> Facilities { get; set; }
    public DbSet<Models.Address> Addresses { get; set; }
    public DbSet<Models.KeyValue> KeyValues { get; set; }

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
