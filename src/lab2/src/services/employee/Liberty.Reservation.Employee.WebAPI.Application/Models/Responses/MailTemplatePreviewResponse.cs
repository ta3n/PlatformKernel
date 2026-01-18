namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;

public record MailTemplatePreviewResponse(
    string? Subject,
    string? Body
);
