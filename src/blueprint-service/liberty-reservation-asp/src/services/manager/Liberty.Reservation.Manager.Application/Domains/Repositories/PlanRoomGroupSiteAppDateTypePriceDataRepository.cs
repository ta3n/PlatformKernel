namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class PlanRoomGroupSiteAppDateTypePriceDataRepository(
    ManagerDataContext dataContext
) : RepositoryBase<PlanRoomGroupSiteAppDateTypePriceData>(dataContext),
    IPlanRoomGroupSiteAppDateTypePriceDataRepository;
