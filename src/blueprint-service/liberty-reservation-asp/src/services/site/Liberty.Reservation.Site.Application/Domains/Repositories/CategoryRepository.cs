namespace Liberty.Reservation.Site.Application.Domains.Repositories;

public class CategoryRepository(
    SiteDataContext dbContext
) : RepositoryBase<Category>(dbContext), ICategoryRepository;
