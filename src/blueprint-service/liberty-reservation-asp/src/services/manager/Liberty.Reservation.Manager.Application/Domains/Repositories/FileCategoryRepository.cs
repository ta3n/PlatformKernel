namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class FileCategoryRepository(
    ManagerDataContext dataContext
) : RepositoryBase<FileCategory>(dataContext), IFileCategoryRepository;
