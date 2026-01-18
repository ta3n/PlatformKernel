namespace Liberty.Reservation.Application.Models.Requests;

public record SaveBookingAggregateFaxAuditLogRequest(
    string FaxNumber,
    string Subject,
    string Result,
    bool IsSuccess,
    string ReservationCode,
    string Body
);
