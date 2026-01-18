using Liberty.ApplicationShared.Extensions;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Templates;

public class IO10103Template(
    IO10011TemplateFormat format
) : BaseTemplate
{
    public IO10011TemplateFormat Format { get; private set; } = format;

    private string? ApplicationName { get; set; }

    private ReservationOperationTypes ReservationOperationType { get; set; }
    private User? User { get; set; }
    private List<Point>? ExpirePoints { get; set; }
    private DateTime DateTime { get; set; }

    public override string Subject
    {
        get
        {
            var args = new FormatDictionary { { "applicationName", ApplicationName } };
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

    public void SetData(
        string applicationName,
        User user,
        List<Point> expirePoints,
        DateTime now
    )
    {
        ApplicationName = applicationName;
        User = user;
        ExpirePoints = expirePoints;
        DateTime = now;
    }

    public override void SetSample()
    {
        var applicationName = "リバティプラン予約システム";

        //
        //
        ApplicationName = applicationName;
    }
}

public class IO10011TemplateFormat
{
    public string Subject { get; set; } = "";

    public string Body { get; set; } = "";
}
