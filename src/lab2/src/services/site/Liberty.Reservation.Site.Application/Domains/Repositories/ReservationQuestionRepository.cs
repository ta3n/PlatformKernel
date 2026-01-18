namespace Liberty.Reservation.Site.Application.Domains.Repositories;

public class ReservationQuestionRepository(
    SiteDataContext dataContext
) : RepositoryBase<ReservationQuestion>(dataContext), IReservationQuestionRepository;
