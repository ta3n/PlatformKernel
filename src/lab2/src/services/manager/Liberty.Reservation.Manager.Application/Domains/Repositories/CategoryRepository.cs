namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class CategoryRepository(
    ManagerDataContext dbContext
) : RepositoryBase<Category>(dbContext), ICategoryRepository;
