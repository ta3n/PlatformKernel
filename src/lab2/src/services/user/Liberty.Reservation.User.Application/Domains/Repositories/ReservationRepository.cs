using Liberty.Reservation.User.Application.Domains.Repositories.Interfaces;
using Liberty.UnitOfWork.Implementations;

namespace Liberty.Reservation.User.Application.Domains.Repositories;

public class ReservationRepository(
    UserDataContext dbContext
) : RepositoryBase<Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation>(dbContext),
    IReservationRepository;
