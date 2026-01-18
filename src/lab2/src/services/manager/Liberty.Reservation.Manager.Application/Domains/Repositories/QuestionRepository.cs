namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class QuestionRepository(
    ManagerDataContext dbContext
) : RepositoryBase<Question>(dbContext), IQuestionRepository;
