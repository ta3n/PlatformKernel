namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class AreaRepository(
    ManagerDataContext dbContext
) : RepositoryBase<Area>(dbContext), IAreaRepository;
