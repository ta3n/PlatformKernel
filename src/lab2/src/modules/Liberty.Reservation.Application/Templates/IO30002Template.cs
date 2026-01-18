using Liberty.ApplicationShared.Extensions;
using Liberty.Reservation.Application.Templates.FormatModels;

namespace Liberty.Reservation.Application.Templates;

public class Io30002Template(
    Io30002TemplateFormat format
) : BaseTemplate
{
    public Io30002TemplateFormat Format { get; private set; } = format;

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
