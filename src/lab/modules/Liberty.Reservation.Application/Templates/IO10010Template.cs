using Liberty.ApplicationShared.Extensions;
using Liberty.Reservation.Application.Constants;

namespace Liberty.Reservation.Application.Templates;

public class IO10010Template(
    IO10010TemplateFormat format
) : BaseTemplate
{
    public IO10010TemplateFormat Format { get; private set; } = format;

    private string? ApplicationName { get; set; }
    private Contexts.DataContexts.Entities.Data.Reservation? Reservation { get; set; }
    private string? ReservationConfirmPassStringFormat { get; set; }

    private ReservationOperationTypes ReservationOperationType { get; set; }

    public override string Subject
    {
        get
        {
            var args = new FormatDictionary
            {
                { "applicationName", ApplicationName },
                { "code", Reservation?.ReservationData?.Code }
            };
            return Format.Subject.FormatByName(args);
        }
    }

    public override string Body
    {
        get
        {
            var data = Reservation?.ReservationData;

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
        ApplicationName = applicationName;
        Reservation = reservation;
        ReservationOperationType = reservationCancelType;
    }

    public override void SetSample()
    {
        var applicationName = "リバティプラン予約システム";
        //
        ApplicationName = applicationName;
    }
}

public class IO10010TemplateFormat
{
    public string Subject { get; set; } = "";

    public string Body { get; set; } = "";
}
