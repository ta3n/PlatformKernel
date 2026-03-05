namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class FacilityPlanRepository(
    ManagerDataContext dataContext
) : RepositoryBase<FacilityPlan>(dataContext), IFacilityPlanRepository;
