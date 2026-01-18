using Liberty.Reservation.Application.Contexts.DataContexts.Entities.AggregateLogs;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Application.Domains.Services;

public class BookingAggregateMailAuditLogService(
    ILogger<BookingAggregateMailAuditLogService> logger,
    IBookingAggregateMailAuditLogRepository bookingAggregateMailAuditLogRepository
) : BaseService<BookingAggregateMailAuditLog>(
        logger,
        bookingAggregateMailAuditLogRepository,
        new BookingAggregateMailAuditLogNotfoundException()
    ),
    IBookingAggregateMailAuditLogService;
