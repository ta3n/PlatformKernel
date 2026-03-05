using System.ComponentModel.DataAnnotations;

namespace Liberty.Reservation.Application.Templates.FormatModels;

public class Io10205TemplateFormat
{
    [StringLength(500)]
    public string Subject { get; set; } = "*";

    [StringLength(5000)]
    public string Body { get; set; } = "*";
}
