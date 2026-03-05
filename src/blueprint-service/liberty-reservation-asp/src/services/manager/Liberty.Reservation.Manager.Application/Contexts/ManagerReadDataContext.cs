namespace Liberty.Reservation.Manager.Application.Contexts;

public class ManagerReadDataContext(
    DbContextOptions<ManagerReadDataContext> options
) : ManagerDataContext(options);
