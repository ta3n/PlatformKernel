namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class OptionItemAppDateRepository(
    ManagerDataContext dbContext
) : RepositoryBase<OptionItemAppDate>(dbContext), IOptionItemAppDateRepository;
