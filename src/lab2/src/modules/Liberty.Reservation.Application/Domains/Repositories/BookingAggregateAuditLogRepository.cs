using Liberty.Reservation.Application.Contexts.DataContexts.Entities.AggregateLogs;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.UnitOfWork.Implementations;
using Microsoft.EntityFrameworkCore;

namespace Liberty.Reservation.Application.Domains.Repositories;

public class BookingAggregateAuditLogRepository(
    DbContext dataContext
) : RepositoryBase<BookingAggregateAuditLog>(dataContext),
    IBookingAggregateAuditLogRepository;
