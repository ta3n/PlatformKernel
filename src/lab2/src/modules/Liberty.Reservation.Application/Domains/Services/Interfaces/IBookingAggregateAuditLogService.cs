using Liberty.Reservation.Application.Contexts.DataContexts.Entities.AggregateLogs;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.Models.Responses;

namespace Liberty.Reservation.Application.Domains.Services.Interfaces;

public interface IBookingAggregateAuditLogService : IBaseService<BookingAggregateAuditLog>
{
    Task<BookingAggregateAuditLog?> FindLastestReservationAsync(
        long? bookingId,
        CancellationToken cancellationToken = default
    );

    Task<List<ChangeOfBookingAuditLogResponse>> DiffRequestBookingAuditLogsAsync(
        BookingAdjustRequest requestNew,
        BookingAdjustRequest requestOld,
        string? languageCode = "ja",
        CancellationToken cancellationToken = default
    );
}
