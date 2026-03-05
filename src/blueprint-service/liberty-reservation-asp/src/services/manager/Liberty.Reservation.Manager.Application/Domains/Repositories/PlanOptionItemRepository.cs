namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class PlanOptionItemRepository(
    ManagerDataContext dataContext
) : RepositoryBase<PlanOptionItem>(dataContext), IPlanOptionItemRepository;
