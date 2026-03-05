namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;

public record MailTemplatePreviewRequest(
    string Format
)
{
    public string? IoType { get; set; }
}
