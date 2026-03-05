namespace Liberty.Reservation.Site.Application.Domains.Services;

public class ReservationRoomGroupAppDatePersonAgeTypeService(
    ILogger<ReservationRoomGroupAppDatePersonAgeTypeService> logger,
    IReservationRoomGroupAppDatePersonAgeTypeRepository reservationRepository
) : BaseServiceRelation<ReservationRoomGroupAppDatePersonAgeType>(logger, reservationRepository),
    IReservationRoomGroupAppDatePersonAgeTypeService;
