using Liberty.Reservation.Application.Contexts.DataContexts;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.UnitOfWork;

namespace Liberty.Reservation.Employee.Application.Contexts;

public class EmployeeDataContext(
    DbContextOptions options
) : AppDbContextBase<EmployeeDataContext>(options)
{
    // Data
    public DbSet<AppDate> AppDates { get; set; }
    public DbSet<AppDateData> AppDateDatas { get; set; }
    public DbSet<AppDateType> AppDateTypes { get; set; }
    public DbSet<Facility> Facilities { get; set; }
    public DbSet<Site> Sites { get; set; }
    public DbSet<FaxService> FaxServices { get; set; }
    public DbSet<SystemConfig> SystemConfigs { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Area> Areas { get; set; }
    public DbSet<Allergen> Allergens { get; set; }
    public DbSet<PersonAgeType> PersonAgeTypes { get; set; }
    public DbSet<ConsumptionTax> ConsumptionTaxs { get; set; }
    public DbSet<MealType> MealTypes { get; set; }
    public DbSet<AlertMessage> AlertMessages { get; set; }

    // Relations
    public DbSet<AppDateAppDateData> AppDateAppDateData { get; set; }
    public DbSet<AppDateAppDateType> AppDateAppDateType { get; set; }
    public DbSet<FacilitySite> FacilitySites { get; set; }
    public DbSet<FacilityFaxService> FacilityFaxServices { get; set; }
    public DbSet<PersonAgeTypeSpaTaxData> PersonAgeTypeSpaTaxDatas { get; set; }
    public DbSet<FacilityCategory> FacilityCategories { get; set; }

    public DbSet<Language> Languages { get; set; }

    protected override void OnModelCreating(
        ModelBuilder modelBuilder
    )
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReservationDataContext).Assembly);
    }
}
