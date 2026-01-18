using Liberty.ApplicationShared.Extensions;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Templates.FormatModels;

namespace Liberty.Reservation.Application.Templates;

public class Io10009Template(
    Io10009TemplateFormat format
) : BaseTemplate
{
    public Io10009TemplateFormat Format { get; private set; } = format;
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
            _ = ReservationOperationType;
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
        ApplicationName = "リバティプラン予約システム";
    }
}
