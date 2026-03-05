namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class RoomGroupSiteRepository(
    ManagerDataContext dbContext
) : RepositoryBase<RoomGroupSite>(dbContext), IRoomGroupSiteRepository;
