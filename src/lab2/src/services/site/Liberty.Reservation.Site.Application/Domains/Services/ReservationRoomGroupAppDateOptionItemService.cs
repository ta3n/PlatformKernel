namespace Liberty.Reservation.Site.Application.Domains.Services;

public class ReservationRoomGroupAppDateOptionItemService(
    ILogger<ReservationRoomGroupAppDateOptionItemService> logger,
    IReservationRoomGroupAppDateOptionItemRepository reservationRepository
) : BaseServiceRelation<ReservationRoomGroupAppDateOptionItem>(logger, reservationRepository),
    IReservationRoomGroupAppDateOptionItemService;
