namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class FileRoomGroupRepository(
    ManagerDataContext dbContext
) : RepositoryBase<FileRoomGroup>(dbContext), IFileRoomGroupRepository;
