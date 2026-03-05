namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class PlanRoomGroupSiteRepository(
    ManagerDataContext dataContext
) : RepositoryBase<PlanRoomGroupSite>(dataContext), IPlanRoomGroupSiteRepository;
