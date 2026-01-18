using Liberty.Cache.Services;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.AggregateLogs;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Application.Domains.Services;

public class BookingAggregateFaxAuditLogService(
    ILogger<BookingAggregateFaxAuditLogService> logger,
    ICacheService cacheService,
    IBookingAggregateFaxAuditLogRepository faxAuditLogRepository
) : BaseService<BookingAggregateFaxAuditLog>(
        logger,
        cacheService,
        faxAuditLogRepository,
        new FaxAuditLogNotFoundException()
    ),
    IBookingAggregateFaxAuditLogService
{
}
