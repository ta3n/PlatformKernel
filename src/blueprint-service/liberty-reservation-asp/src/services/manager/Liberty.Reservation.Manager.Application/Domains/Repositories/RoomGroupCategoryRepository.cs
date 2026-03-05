namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class RoomGroupCategoryRepository(
    ManagerDataContext dbContext
) : RepositoryBase<RoomGroupCategory>(dbContext), IRoomGroupCategoryRepository;
