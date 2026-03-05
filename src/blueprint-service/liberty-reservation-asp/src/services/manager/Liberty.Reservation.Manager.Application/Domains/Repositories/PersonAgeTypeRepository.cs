namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class PersonAgeTypeRepository(
    ManagerDataContext dataContext
) : RepositoryBase<PersonAgeType>(dataContext), IPersonAgeTypeRepository;
