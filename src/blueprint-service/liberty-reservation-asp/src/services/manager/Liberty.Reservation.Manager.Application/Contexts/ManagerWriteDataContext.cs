namespace Liberty.Reservation.Manager.Application.Contexts;

public class ManagerWriteDataContext(
    DbContextOptions<ManagerWriteDataContext> options
) : ManagerDataContext(options);
