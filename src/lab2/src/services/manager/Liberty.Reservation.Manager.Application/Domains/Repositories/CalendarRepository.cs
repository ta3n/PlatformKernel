namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class CalendarRepository(
    ManagerDataContext dataContext
) : RepositoryBase<Calendar>(dataContext), ICalendarRepository;
