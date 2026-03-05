using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.UnitOfWork.Implementations;
using Microsoft.EntityFrameworkCore;
using Order = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Order;

namespace Liberty.Reservation.Application.Domains.Repositories;

public class BookingOrderRepository(
    DbContext dbContext
) : RepositoryBase<Order>(dbContext), IBookingOrderRepository;
