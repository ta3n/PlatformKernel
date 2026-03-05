namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class ReservationRepository(
    ManagerDataContext dbContext
) : RepositoryBase<Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation>(dbContext),
    IReservationRepository;
