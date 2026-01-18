namespace Liberty.Reservation.Site.Application.Domains.Repositories;

public class FilePlanRepository(
    SiteDataContext dataContext
) : RepositoryBase<FilePlan>(dataContext), IFilePlanRepository;
