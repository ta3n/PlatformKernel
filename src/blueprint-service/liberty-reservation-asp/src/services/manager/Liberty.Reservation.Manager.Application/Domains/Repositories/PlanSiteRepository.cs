namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class PlanSiteRepository(
    ManagerDataContext dataContext
) : RepositoryBase<PlanSite>(dataContext), IPlanSiteRepository;
