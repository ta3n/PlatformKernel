namespace Liberty.Reservation.Site.Application.Domains.Repositories;

public class PlanQuestionRepository(
    SiteDataContext dataContext
) : RepositoryBase<PlanQuestion>(dataContext), IPlanQuestionRepository;
