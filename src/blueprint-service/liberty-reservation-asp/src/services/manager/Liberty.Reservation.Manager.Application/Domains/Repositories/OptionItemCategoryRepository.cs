namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class OptionItemCategoryRepository(
    ManagerDataContext dbContext
) : RepositoryBase<OptionItemCategory>(dbContext), IOptionItemCategoryRepository;
