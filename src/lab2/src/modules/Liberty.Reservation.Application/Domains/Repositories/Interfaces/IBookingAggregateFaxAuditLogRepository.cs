using Liberty.Reservation.Application.Contexts.DataContexts.Entities.AggregateLogs;
using Liberty.UnitOfWork.Abstractions;

namespace Liberty.Reservation.Application.Domains.Repositories.Interfaces;

public interface IBookingAggregateFaxAuditLogRepository : IRepositoryBase<BookingAggregateFaxAuditLog>;
