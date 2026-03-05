namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class PlanQuestionRepository(
    ManagerDataContext dataContext
) : RepositoryBase<PlanQuestion>(dataContext), IPlanQuestionRepository;
