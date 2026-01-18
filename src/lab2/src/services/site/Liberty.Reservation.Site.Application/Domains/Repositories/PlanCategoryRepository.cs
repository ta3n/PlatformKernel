namespace Liberty.Reservation.Site.Application.Domains.Repositories;

public class PlanCategoryRepository(
    SiteDataContext dataContext
) : RepositoryBase<PlanCategory>(dataContext), IPlanCategoryRepository;
