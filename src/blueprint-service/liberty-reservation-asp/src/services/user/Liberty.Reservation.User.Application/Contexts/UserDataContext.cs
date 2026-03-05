using Liberty.Reservation.Application.Contexts.DataContexts;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.UnitOfWork;

namespace Liberty.Reservation.User.Application.Contexts;

public class UserDataContext(
    DbContextOptions options
) : AppDbContextBase<UserDataContext>(options)
{
    // Data
    public DbSet<Facility> Facilities { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<GmoPaymentResultRequest> GmoPaymentResultRequests { get; set; }
    public DbSet<Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation> Reservations { get; set; }

    // Relations
    public DbSet<AppDateAppDateData> AppDateAppDateData { get; set; }

    protected override void OnModelCreating(
        ModelBuilder modelBuilder
    )
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReservationDataContext).Assembly);
    }
}
