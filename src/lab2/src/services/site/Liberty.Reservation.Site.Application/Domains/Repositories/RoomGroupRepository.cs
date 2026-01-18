namespace Liberty.Reservation.Site.Application.Domains.Repositories;

public class RoomGroupRepository(
    SiteDataContext dataContext
) : RepositoryBase<RoomGroup>(dataContext), IRoomGroupRepository;
