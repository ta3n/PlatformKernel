using Liberty.Reservation.Site.Application.Exceptions;

namespace Liberty.Reservation.Site.Application.Domains.Services;

public class ReservationService(
    ILogger<ReservationService> logger,
    IReservationRepository reservationRepository
) : BaseService<Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation>(
        logger,
        reservationRepository,
        new FacilityNotfoundException()
    ),
    IReservationService;
