using Liberty.ApplicationShared.Extensions;
using Liberty.Reservation.Application.Templates.FormatModels;

namespace Liberty.Reservation.Application.Templates;

public class Io30001Template(
    Io30001TemplateFormat format
) : BaseTemplate
{
    private Io30001TemplateFormat Format { get; set; } = format;

    public override string Subject
    {
        get
        {
            var args = new FormatDictionary();
            return Format.Subject.FormatByName(args);
        }
    }

    public override string Body
    {
        get
        {
            var args = new FormatDictionary();
            return Format.Body.FormatByName(args);
        }
    }
}
