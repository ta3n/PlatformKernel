namespace Liberty.Reservation.Site.Application.Domains.Repositories;

public class ReservationRoomGroupAppDateOptionItemRepository(
    SiteDataContext dataContext
) : RepositoryBase<ReservationRoomGroupAppDateOptionItem>(dataContext), IReservationRoomGroupAppDateOptionItemRepository;
