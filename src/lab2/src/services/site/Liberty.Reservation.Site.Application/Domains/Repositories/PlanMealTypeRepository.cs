namespace Liberty.Reservation.Site.Application.Domains.Repositories;

public class PlanMealTypeRepository(
    SiteDataContext dataContext
) : RepositoryBase<PlanMealType>(dataContext), IPlanMealTypeRepository;
