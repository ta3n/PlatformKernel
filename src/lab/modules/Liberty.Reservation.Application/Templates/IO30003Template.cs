using Liberty.ApplicationShared.Extensions;

namespace Liberty.Reservation.Application.Templates;

public class IO30003Template(
    IO30003TemplateFormat format
) : BaseTemplate
{
    public IO30003TemplateFormat Format { get; private set; } = format;

    private Contexts.DataContexts.Entities.Data.Reservation? Reservation { get; set; }

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

public class IO30003TemplateFormat
{
    public string Subject { get; set; } = "";

    public string Body { get; set; } = "";
}
