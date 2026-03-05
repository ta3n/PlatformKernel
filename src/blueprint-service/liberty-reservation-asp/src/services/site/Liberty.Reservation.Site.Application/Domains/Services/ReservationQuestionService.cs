namespace Liberty.Reservation.Site.Application.Domains.Services;

public class ReservationQuestionService(
    ILogger<ReservationQuestionService> logger,
    IReservationQuestionRepository reservationQuestionRepository
) : BaseServiceRelation<ReservationQuestion>(logger, reservationQuestionRepository), IReservationQuestionService;
