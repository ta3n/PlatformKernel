using Liberty.UnitOfWork.Abstractions;

namespace Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;

public interface IReservationRepository
    : IRepositoryBase<Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation>;
