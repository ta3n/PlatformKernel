namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class PlanRoomGroupSitePersonAgeTypeRepository(
    ManagerDataContext dataContext
) : RepositoryBase<PlanRoomGroupSitePersonAgeType>(dataContext), IPlanRoomGroupSitePersonAgeTypeRepository;
