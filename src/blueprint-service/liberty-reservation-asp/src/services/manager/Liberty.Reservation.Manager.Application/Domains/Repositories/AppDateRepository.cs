namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class AppDateRepository(
    ManagerDataContext dbContext
) : RepositoryBase<AppDate>(dbContext), IAppDateRepository;
