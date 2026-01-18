using Liberty.Reservation.Application.Contexts.DataContexts;
using Liberty.UnitOfWork;
using File = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.File;

namespace Liberty.Reservation.Site.Application.Contexts;

public class SiteDataContext(
    DbContextOptions options
) : AppDbContextBase<SiteDataContext>(options)
{
    // Data
    public DbSet<Facility> Facilities { get; set; }
    public DbSet<Reservation.Application.Contexts.DataContexts.Entities.Data.Site> Sites { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Plan> Plans { get; set; }
    public DbSet<File> Files { get; set; }

    // Relations
    public DbSet<AppDateAppDateData> AppDateAppDateData { get; set; }
    public DbSet<FacilityFile> FacilityFiles { get; set; }
    public DbSet<FacilityPersonAgeType> FacilityPersonAgeTypes { get; set; }
    public DbSet<FacilityQuestion> FacilityQuestions { get; set; }
    public DbSet<FacilityCategory> FacilityCategories { get; set; }
    public DbSet<FacilityCancellation> FacilityCancellations { get; set; }
    public DbSet<FacilitySite> FacilitySites { get; set; }

    protected override void OnModelCreating(
        ModelBuilder modelBuilder
    )
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReservationDataContext).Assembly);
    }
}
