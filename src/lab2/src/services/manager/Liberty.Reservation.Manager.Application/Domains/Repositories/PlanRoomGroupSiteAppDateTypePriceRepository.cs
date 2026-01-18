namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class PlanRoomGroupSiteAppDateTypePriceRepository(
    ManagerDataContext dataContext
) : RepositoryBase<PlanRoomGroupSitePersonAgeType>(dataContext), IPlanRoomGroupSitePersonAgeTypeRepository;
