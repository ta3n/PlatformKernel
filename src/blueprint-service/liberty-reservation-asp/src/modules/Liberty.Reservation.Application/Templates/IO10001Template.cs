using Liberty.ApplicationShared.Extensions;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Liberty.Reservation.Application.Templates.FormatModels;
using static Liberty.ApplicationShared.Domains.Services.Mails.SmtpMailService;

namespace Liberty.Reservation.Application.Templates;

public class Io10001Template(
    Io10001TemplateFormat format,
    SmtpMailSetting smtpMailSetting
) : BaseTemplate
{
    private SmtpMailSetting Settings { get; set; } = smtpMailSetting;
    private Io10001TemplateFormat Format { get; set; } = format;
    private string? ReservationConfirmPassStringFormat { get; set; }

    public override string Subject
    {
        get
        {
            var data = Reservation?.BookingData;

            var args = new FormatDictionary
            {
                { "facilityName", data?.Facility.Name },
                { "code", Reservation?.Code ?? string.Empty }
            };
            return Format.Subject.FormatByName(args);
        }
    }

    public override string Body
    {
        get
        {
            var data = Reservation?.BookingData;

            var args = new FormatDictionary
            {
                { "facilityName", data?.Facility.Name },
                { "applicationName", ApplicationName },
                { "code", Reservation?.Code },
                {
                    "url", URL != null && Reservation?.Id != null
                        ? URL.Replace("{guestCode}", $"{ReservationConfirmPassStringFormat}")
                        : null
                }
            };
            return Format.Body.FormatByName(args);
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
        var applicationName = Settings.ApplicationNamePreview ?? "サンプルアプリ名";
        const string reservationConfirmPassStringFormat = "{0}_{1}";

        var reservation = new Contexts.DataContexts.Entities.Data.Reservation
        {
            Id = -1,
            Code = "112233"
        };

        var reservationData = new BookingData
        {
            Facility = new FacilityData(),
            Plan = new PlanData(),
            RoomGroup = new RoomGroupData(),
            Site = new SiteData()
        };

        reservationData.Facility.Name = Settings.FacilityNamePreview ?? "サンプル荘";

        reservation.BookingData = reservationData;

        SetData(reservation, applicationName, reservationConfirmPassStringFormat);
    }
}
