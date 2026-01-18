namespace Liberty.Reservation.Booking.Worker.Application.Models.Requests;

public interface IEmailSentAuditLogRequest;

public record EmailSentAuditLogRequest(
    string JsonData
);
