namespace Liberty.Reservation.Application.Models;

public record SaveFaxAuditLogModel(
    string FaxNumber,
    string Subject,
    string Result,
    bool IsSuccess,
    long BookingId,
    string Body
);
