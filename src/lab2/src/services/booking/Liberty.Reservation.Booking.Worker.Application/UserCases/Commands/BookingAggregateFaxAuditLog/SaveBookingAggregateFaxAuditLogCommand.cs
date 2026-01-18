using Liberty.Reservation.Application.Models.Requests;

namespace Liberty.Reservation.Booking.Worker.Application.UserCases.Commands.BookingAggregateFaxAuditLog;

public record SaveBookingAggregateFaxAuditLogCommand : UpdateCommandBase<
    SaveBookingAggregateFaxAuditLogRequest,
    bool
>;
