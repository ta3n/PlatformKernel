using Liberty.ApplicationShared.Extensions;

namespace Liberty.Reservation.Application.Templates;

public class IO30001Template(
    IO30001TemplateFormat format
) : BaseTemplate
{
    public IO30001TemplateFormat Format { get; private set; } = format;

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

public class IO30001TemplateFormat
{
    public string Subject { get; set; } = "";

    public string Body { get; set; } = "";
}
