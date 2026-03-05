using Liberty.Reservation.Application.Domains.Repositories.Interfaces;

namespace Liberty.Reservation.Site.Application.Domains.Repositories;

public class BookingPlanRoomGroupSiteAppDateTypePriceRepository(
    SiteDataContext dataContext
) : RepositoryBase<PlanRoomGroupSitePersonAgeType>(dataContext), IBookingPlanRoomGroupSitePersonAgeTypeRepository;
