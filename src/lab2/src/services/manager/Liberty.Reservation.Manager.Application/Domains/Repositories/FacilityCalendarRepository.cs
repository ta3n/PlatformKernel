namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class FacilityCalendarRepository(
    ManagerDataContext dataContext
) : RepositoryBase<FacilityCalendar>(dataContext), IFacilityCalendarRepository;
