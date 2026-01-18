using Liberty.ApplicationShared.Extensions;
using Liberty.Reservation.Application.Constants;

namespace Liberty.Reservation.Application.Templates;

public class IO10009Template(
    IO10009TemplateFormat format
) : BaseTemplate
{
    public IO10009TemplateFormat Format { get; private set; } = format;

    private string? ApplicationName { get; set; }
    private Contexts.DataContexts.Entities.Data.Reservation? Reservation { get; set; }
    private ReservationOperationTypes ReservationOperationType { get; set; }

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
            var args = new FormatDictionary();

            return Format.Body.FormatByName(args);
        }
    }

    public void SetData(
        Contexts.DataContexts.Entities.Data.Reservation reservation,
        string applicationName,
        ReservationOperationTypes reservationCancelType
    )
    {
        Reservation = reservation;
        ApplicationName = applicationName;
        ReservationOperationType = reservationCancelType;
    }

    public override void SetSample()
    {
        var applicationName = "リバティプラン予約システム";

        //
        //
        ApplicationName = applicationName;
    }
}

public class IO10009TemplateFormat
{
    public string Subject { get; set; } = "";

    public string Body { get; set; } = "";
}
