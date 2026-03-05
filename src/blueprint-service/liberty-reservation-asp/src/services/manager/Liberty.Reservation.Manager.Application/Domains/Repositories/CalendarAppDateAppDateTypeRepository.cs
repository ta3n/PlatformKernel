namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class CalendarAppDateAppDateTypeRepository(
    ManagerDataContext dataContext
) : RepositoryBase<CalendarAppDateAppDateType>(dataContext), ICalendarAppDateAppDateTypeRepository;
