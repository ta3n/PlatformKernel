using Liberty.UnitOfWork.Abstractions;
using Order = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Order;

namespace Liberty.Reservation.Application.Domains.Repositories.Interfaces;

public interface IBookingOrderRepository : IRepositoryBase<Order>;
