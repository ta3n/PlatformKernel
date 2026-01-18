using Liberty.ApplicationShared.Extensions;

namespace Liberty.Reservation.Application.Templates;

public class IO30002Template(
    IO30002TemplateFormat format
) : BaseTemplate
{
    public IO30002TemplateFormat Format { get; private set; } = format;

    public Contexts.DataContexts.Entities.Data.Reservation? Reservation { get; set; }

    public override string Subject
    {
        get
        {
            var args = new FormatDictionary { };
            return Format.Subject.FormatByName(args);
        }
    }

    public override string Body
    {
        get
        {
            var args = new FormatDictionary { };
            return Format.Body.FormatByName(args);
        }
    }

    public override void SetSample()
    {
    }
}

public class IO30002TemplateFormat
{
    public string Subject { get; set; } = "";

    public string Body { get; set; } = "";
}
