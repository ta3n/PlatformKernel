namespace Liberty.Reservation.Site.Application.Domains.Repositories;

public class SiteRepository(
    SiteDataContext dataContext
) : RepositoryBase<Reservation.Application.Contexts.DataContexts.Entities.Data.Site>(dataContext),
    ISiteRepository;
