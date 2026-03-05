namespace Liberty.Reservation.Site.Application.Domains.Repositories;

public class PlanRoomGroupSiteRepository(
    SiteDataContext dataContext
) : RepositoryBase<PlanRoomGroupSite>(dataContext), IPlanRoomGroupSiteRepository;
