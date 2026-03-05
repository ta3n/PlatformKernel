namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;

public record MailTemplateUpdateRequest(
    string Format
)
{
    public string? IoType { get; set; }
}
