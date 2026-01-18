namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class PlanCategoryRepository(
    ManagerDataContext dataContext
) : RepositoryBase<PlanCategory>(dataContext), IPlanCategoryRepository;
