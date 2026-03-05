namespace Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;

public interface IReservationRepository
    : IRepositoryBase<Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation>;
