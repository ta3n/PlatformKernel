namespace Liberty.Reservation.Site.Application.Domains.Repositories;

public class PlanRepository(
    SiteDataContext dataContext
) : RepositoryBase<Plan>(dataContext), IPlanRepository;
