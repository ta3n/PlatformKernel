namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class PersonAgeTypeSpaTaxDataRepository(
    ManagerDataContext dataContext
) : RepositoryBase<PersonAgeTypeSpaTaxData>(dataContext), IPersonAgeTypeSpaTaxDataRepository;
