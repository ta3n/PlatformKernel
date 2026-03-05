using Liberty.Reservation.Application.Domains.Repositories.Interfaces;

namespace Liberty.Reservation.Site.Application.Domains.Services;

public class ReservationPlanRoomGroupAppDateService(
    ILogger<ReservationPlanRoomGroupAppDateService> logger,
    IBookingReservationPlanRoomGroupAppDateRepository bookingReservationRepository
) : BaseServiceRelation<ReservationPlanRoomGroupAppDate>(logger, bookingReservationRepository), IReservationPlanRoomGroupAppDateService;
