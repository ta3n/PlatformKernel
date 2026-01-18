namespace Liberty.Reservation.Manager.Application;

public class ManagerUnitOfWork(
    DbContext context
) : BaseUnitOfWork(context), IManagerUnitOfWork;
