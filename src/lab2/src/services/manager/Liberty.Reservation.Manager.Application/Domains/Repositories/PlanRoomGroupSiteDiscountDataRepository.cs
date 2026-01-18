namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class PlanRoomGroupSiteDiscountDataRepository(
    ManagerDataContext dataContext
) : RepositoryBase<PlanRoomGroupSiteDiscountData>(dataContext), IPlanRoomGroupSiteDiscountDataRepository;
