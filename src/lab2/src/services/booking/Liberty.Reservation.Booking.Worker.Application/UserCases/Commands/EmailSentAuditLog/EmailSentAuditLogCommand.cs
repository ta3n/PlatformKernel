using Liberty.Reservation.Booking.Worker.Application.Models.Requests;

namespace Liberty.Reservation.Booking.Worker.Application.UserCases.Commands.EmailSentAuditLog;

public record EmailSentAuditLogCommand : ActionCommandBase<
    EmailSentAuditLogRequest,
    bool
>;
