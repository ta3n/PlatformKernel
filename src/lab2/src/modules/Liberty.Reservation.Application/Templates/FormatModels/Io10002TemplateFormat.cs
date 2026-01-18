using System.ComponentModel.DataAnnotations;

namespace Liberty.Reservation.Application.Templates.FormatModels;

public class Io10002TemplateFormat
{
    [StringLength(500)]
    public string Subject { get; set; } = "*IO10002 ご予約成立";

    [StringLength(5000)]
    public string Body { get; set; } = "*IO10002 本メールは宿泊施設向けの内容です。\n本予約は予約成立されました";

    [StringLength(500)]
    public string Url { get; set; } = "http://";
}
