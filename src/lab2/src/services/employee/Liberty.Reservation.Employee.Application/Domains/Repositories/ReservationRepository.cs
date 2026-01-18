using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.UnitOfWork.Implementations;

namespace Liberty.Reservation.Employee.Application.Domains.Repositories;

public class ReservationRepository(
    EmployeeDataContext dbContext
) : RepositoryBase<Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation>(dbContext),
    IReservationRepository;
