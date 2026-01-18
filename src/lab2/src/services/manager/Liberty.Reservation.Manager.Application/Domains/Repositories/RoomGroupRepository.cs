namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class RoomGroupRepository(
    ManagerDataContext dbContext
) : RepositoryBase<RoomGroup>(dbContext), IRoomGroupRepository;
