namespace Liberty.Reservation.Site.Application.Domains.Repositories;

public class ReservationRoomGroupAppDatePersonAgeTypeRepository(
    SiteDataContext dataContext
) : RepositoryBase<ReservationRoomGroupAppDatePersonAgeType>(dataContext), IReservationRoomGroupAppDatePersonAgeTypeRepository;
