using Liberty.Reservation.Application.Templates;

namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;

public record MailTemplateResponse(
    string? Subject,
    string? Body,
    string? Url = null
)
{
    public string? IoType { get; set; }
    public List<MailTemplateParameter> Parameters { get; set; } = [];
}
