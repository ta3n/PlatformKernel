using Liberty.Reservation.Application.Contexts.DataContexts.Entities.AggregateLogs;

namespace Liberty.Reservation.Manager.Application.Contexts;

public class ManagerDataContext(
    DbContextOptions options
) : AppDbContextBase<ManagerDataContext>(options)
{
    // Data
    public DbSet<Facility> Facilities { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<RoomGroup> RoomGroups { get; set; }
    public DbSet<OptionItem> OptionItems { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<BedType> BedTypes { get; set; }
    public DbSet<AppDateType> AppDateTypes { get; set; }
    public DbSet<Calendar> Calendars { get; set; }
    public DbSet<CancellationData> CancellationDatas { get; set; }
    public DbSet<Cancellation> Cancellations { get; set; }
    public DbSet<OrderReservation> OrderReservations { get; set; }
    public DbSet<CancellationCancellationData> CancellationCancellationDatas { get; set; }

    // Relations
    public DbSet<FacilityOptionItem> FacilityOptionItems { get; set; }
    public DbSet<FacilityRoomGroup> FacilityRoomGroups { get; set; }
    public DbSet<OptionItemCategory> OptionItemCategories { get; set; }
    public DbSet<OptionItemQuestion> OptionItemQuestions { get; set; }
    public DbSet<RoomGroupBedType> RoomGroupBedTypes { get; set; }
    public DbSet<AppDateAppDateData> AppDateAppDateDatas { get; set; }
    public DbSet<CalendarAppDateAppDateType> CalendarAppDateAppDateTypes { get; set; }
    public DbSet<FacilityAppDateType> FacilityAppDateTypes { get; set; }
    public DbSet<FacilityCalendar> FacilityCalendars { get; set; }
    public DbSet<FacilityFile> FacilityFiles { get; set; }
    public DbSet<FacilityPersonAgeType> FacilityPersonAgeTypes { get; set; }
    public DbSet<FileCategory> FileCategories { get; set; }
    public DbSet<PersonAgeTypeSpaTaxData> PersonAgeTypeSpaTaxDatas { get; set; }
    public DbSet<FacilityQuestion> FacilityQuestions { get; set; }
    public DbSet<AppDateAppDateData> AppDateAppDateData { get; set; }
    public DbSet<FacilityCategory> FacilityCategories { get; set; }
    public DbSet<FacilityCancellation> FacilityCancellations { get; set; }
    public DbSet<BookingAggregateFaxAuditLog> FaxAuditLogs { get; set; }

    protected override void OnModelCreating(
        ModelBuilder modelBuilder
    )
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReservationDataContext).Assembly);
    }
}
