namespace Liberty.Reservation.Site.Application.Domains.Repositories;

public class ReservationRepository(
    SiteDataContext dataContext
) : RepositoryBase<Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation>(dataContext), IReservationRepository;
