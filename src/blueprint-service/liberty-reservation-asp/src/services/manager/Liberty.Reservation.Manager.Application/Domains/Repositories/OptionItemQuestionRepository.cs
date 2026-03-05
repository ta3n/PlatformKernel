namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class OptionItemQuestionRepository(
    ManagerDataContext dbContext
) : RepositoryBase<OptionItemQuestion>(dbContext), IOptionItemQuestionRepository;
