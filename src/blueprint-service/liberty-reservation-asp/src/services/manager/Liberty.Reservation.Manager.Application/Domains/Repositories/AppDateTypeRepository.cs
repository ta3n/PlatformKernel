namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class AppDateTypeRepository(
    ManagerDataContext dataContext
) : RepositoryBase<AppDateType>(dataContext), IAppDateTypeRepository;
