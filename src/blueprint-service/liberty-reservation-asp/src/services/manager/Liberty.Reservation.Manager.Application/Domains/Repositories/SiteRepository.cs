namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class SiteRepository(
    ManagerDataContext dbContext
) : RepositoryBase<Site>(dbContext), ISiteRepository;
