namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class PlanRepository(
    ManagerDataContext dataContext
) : RepositoryBase<Plan>(dataContext), IPlanRepository;
