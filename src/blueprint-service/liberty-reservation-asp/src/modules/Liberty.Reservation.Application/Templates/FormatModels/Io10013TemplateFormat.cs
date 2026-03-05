using System.ComponentModel.DataAnnotations;

namespace Liberty.Reservation.Application.Templates.FormatModels;

public class Io10013TemplateFormat
{
    [StringLength(500)]
    public string Subject { get; set; } = string.Empty;

    [StringLength(5000)]
    public string Body { get; set; } = string.Empty;
}
