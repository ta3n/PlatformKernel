namespace Liberty.Reservation.Site.Application.Domains.Repositories;

public class SystemConfigRepository(
    SiteDataContext dbContext
) : RepositoryBase<SystemConfig>(dbContext), ISystemConfigRepository;
