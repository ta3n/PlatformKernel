using Liberty.Reservation.Application.Domains.Repositories.Interfaces;

namespace Liberty.Reservation.Site.Application.Domains.Repositories;

public class BookingPlanRoomGroupSiteAppDateRepository(
    SiteDataContext dataContext
) : RepositoryBase<PlanRoomGroupSiteAppDate>(dataContext), IBookingPlanRoomGroupSiteAppDateRepository;
