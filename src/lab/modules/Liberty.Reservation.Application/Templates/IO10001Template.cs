using Liberty.ApplicationShared.Extensions;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;

namespace Liberty.Reservation.Application.Templates;

public class IO10001Template(
    IO10001TemplateFormat format
) : BaseTemplate
{
    public IO10001TemplateFormat Format { get; private set; } = format;

    private string? ApplicationName { get; set; }
    private Contexts.DataContexts.Entities.Data.Reservation? Reservation { get; set; }
    private string? ReservationConfirmPassStringFormat { get; set; }

    public override string Subject
    {
        get
        {
            var args = new FormatDictionary
            {
                { "applicationName", ApplicationName ?? string.Empty },
                { "code", Reservation?.ReservationData?.Code ?? string.Empty }
            };
            return Format.Subject.FormatByName(args);
        }
    }

    public override string Body
    {
        get
        {
            var data = Reservation?.ReservationData;

            var args = new FormatDictionary
            {
                { "facilityName", data?.Facility?.Name },
                { "url", Url },
                { "applicationName", ApplicationName }
            };
            return Format.Body.FormatByName(args);
        }
    }

    public string Url
    {
        get
        {
            var id = Reservation?.Id;
            var code = Reservation?.ReservationData?.Code;

            var passString = Contexts.DataContexts.Entities.Data.Reservation.CreatePassString(
                ReservationConfirmPassStringFormat,
                Reservation?.Id,
                Reservation?.ReservationData?.Code
            );

            var args = new FormatDictionary
            {
                { "id", id },
                { "code", code },
                { "passString", passString }
            };
            return Format.Url.FormatByName(args);
        }
    }

    public void SetData(
        Contexts.DataContexts.Entities.Data.Reservation reservation,
        string applicationName,
        string reservationConfirmPassStringFormat
    )
    {
        ApplicationName = applicationName;
        Reservation = reservation;
        ReservationConfirmPassStringFormat = reservationConfirmPassStringFormat;
    }

    public override void SetSample()
    {
        var applicationName = "";
        var reservationConfirmPassStringFormat = "{0}_{1}";

        var reservation = new Contexts.DataContexts.Entities.Data.Reservation { Id = -1 };

        var reservationData = new ReservationData
        {
            Facility = new ReservationFacilityData(),
            Code = "112233"
        };

        reservationData.Facility.Name = "サンプル荘";

        reservation.ReservationData = reservationData;

        SetData(reservation, applicationName, reservationConfirmPassStringFormat);
    }
}

public class IO10001TemplateFormat
{
    public string Subject { get; set; } = "";

    public string Body { get; set; } = "";

    public string Url { get; set; } = "";
}
