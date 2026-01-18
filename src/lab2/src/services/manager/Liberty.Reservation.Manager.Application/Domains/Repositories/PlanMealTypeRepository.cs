namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class PlanMealTypeRepository(
    ManagerDataContext dataContext
) : RepositoryBase<PlanMealType>(dataContext), IPlanMealTypeRepository;
