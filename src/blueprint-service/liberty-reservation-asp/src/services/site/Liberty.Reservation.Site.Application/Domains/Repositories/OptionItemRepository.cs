namespace Liberty.Reservation.Site.Application.Domains.Repositories;

public class OptionItemRepository(
    SiteDataContext dataContext
) : RepositoryBase<OptionItem>(dataContext), IOptionItemRepository;
