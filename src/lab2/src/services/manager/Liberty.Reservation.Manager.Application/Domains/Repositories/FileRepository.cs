using File = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.File;

namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class FileRepository(
    ManagerDataContext dbContext
) : RepositoryBase<File>(dbContext), IFileRepository;
