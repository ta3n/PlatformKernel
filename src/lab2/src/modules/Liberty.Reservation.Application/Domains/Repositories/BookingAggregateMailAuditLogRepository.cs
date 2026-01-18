using Liberty.Reservation.Application.Contexts.DataContexts.Entities.AggregateLogs;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.UnitOfWork.Implementations;
using Microsoft.EntityFrameworkCore;

namespace Liberty.Reservation.Application.Domains.Repositories;

public class BookingAggregateMailAuditLogRepository(
    DbContext dbContext
) : RepositoryBase<BookingAggregateMailAuditLog>(dbContext), IBookingAggregateMailAuditLogRepository;
