namespace Liberty.Reservation.Site.Application.Domains.Repositories;

public class FacilityPlanRepository(
    SiteDataContext dataContext
) : RepositoryBase<FacilityPlan>(dataContext), IFacilityPlanRepository;
