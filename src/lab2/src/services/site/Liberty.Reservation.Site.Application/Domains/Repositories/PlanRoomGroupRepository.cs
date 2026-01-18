namespace Liberty.Reservation.Site.Application.Domains.Repositories;

public class PlanRoomGroupRepository(
    SiteDataContext dataContext
) : RepositoryBase<PlanRoomGroup>(dataContext), IPlanRoomGroupRepository;
