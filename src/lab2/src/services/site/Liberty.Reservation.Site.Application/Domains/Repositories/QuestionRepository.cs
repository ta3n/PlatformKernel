namespace Liberty.Reservation.Site.Application.Domains.Repositories;

public class QuestionRepository(
    SiteDataContext dataContext
) : RepositoryBase<Question>(dataContext), IQuestionRepository;
