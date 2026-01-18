namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class OptionItemRepository(
    ManagerDataContext dbContext
) : RepositoryBase<OptionItem>(dbContext), IOptionItemRepository;
