using File = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.File;

namespace Liberty.Reservation.Site.Application.Domains.Repositories;

public class MediaRepository(
    SiteDataContext dbContext
) : RepositoryBase<File>(dbContext), IMediaRepository;
