namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class PlanRoomGroupSitePriceDataRepository(
    ManagerDataContext dataContext
) : RepositoryBase<PlanRoomGroupSitePriceData>(dataContext),
    IPlanRoomGroupSitePriceDataRepository;
