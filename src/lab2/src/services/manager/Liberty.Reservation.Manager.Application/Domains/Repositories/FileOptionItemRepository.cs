namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class FileOptionItemRepository(
    ManagerDataContext dbContext
) : RepositoryBase<FileOptionItem>(dbContext), IFileOptionItemRepository;
