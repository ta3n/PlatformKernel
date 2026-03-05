namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;

public record MailTemplateSendRequest(
    string Subject,
    string Body,
    string ToEmail
);
