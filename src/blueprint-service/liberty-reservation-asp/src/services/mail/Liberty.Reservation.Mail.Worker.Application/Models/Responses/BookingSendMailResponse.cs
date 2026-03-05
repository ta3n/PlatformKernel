namespace Liberty.Reservation.Mail.Worker.Application.Models.Responses;

public record BookingSendMailResponse(
    long[] Processed,
    long[] Errors
);
