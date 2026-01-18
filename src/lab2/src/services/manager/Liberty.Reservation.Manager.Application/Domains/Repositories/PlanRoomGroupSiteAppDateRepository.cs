namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class PlanRoomGroupSiteAppDateRepository(
    ManagerDataContext dataContext
) : RepositoryBase<PlanRoomGroupSiteAppDate>(dataContext), IPlanRoomGroupSiteAppDateRepository;
