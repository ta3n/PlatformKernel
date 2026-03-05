using System.Text;
using Liberty.ApplicationShared.Extensions;
using Liberty.ApplicationShared.Utils;
using Liberty.Entity.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Templates.FormatModels;
using static Liberty.ApplicationShared.Domains.Services.Mails.SmtpMailService;

namespace Liberty.Reservation.Application.Templates;

public class Io10007Template(
    Io10007TemplateFormat format,
    SmtpMailSetting smtpMailSetting
) : BaseTemplate
{
    private SmtpMailSetting Settings { get; set; } = smtpMailSetting;
    private Io10007TemplateFormat Format { get; set; } = format;

    private ReservationOperationTypes ReservationOperationType { get; set; }
    private Contexts.DataContexts.Entities.Data.Reservation? OldReservation { get; set; }

    public override string Subject
    {
        get
        {
            var rootCode = Reservation?.BookingData?.RootCode ?? string.Empty;
            var args = new FormatDictionary
            {
                { "applicationName", ApplicationName },
                { "code", rootCode }
            };
            return Format.Subject.FormatByName(args);
        }
    }

    public override string Body
    {
        get
        {
            _ = ReservationOperationType;
            _ = OldReservation;
            var data = Reservation?.BookingData;

            var formattedDate = DateTimeFormatWithOffset(
                Reservation?.ReservationDateTime ?? DateTime.UtcNow,
                Reservation?.BookingData?.TimeZoneOffset ?? 0
            );

            var useDay = Reservation?.BookingData?.Plan.DayUse ?? false;
            var checkOutDate = useDay
                ? $"{AppDate.GetDateTime(Reservation?.CheckInDate):yyyy/MM/dd}"
                : $"{AppDate.GetDateTime(Reservation?.CheckInDate)?.AddDays(Reservation?.RestNumber ?? 0):yyyy/MM/dd}";

            var rootCode = Reservation?.BookingData?.RootCode ?? string.Empty;
            var args = new FormatDictionary
            {
                { "facilityName", data?.Facility.Name },
                { "facilityAddress", data?.Facility.Address },
                { "actor", ReservationOperationType == ReservationOperationTypes.Facility ? "施設管理者" : "予約者" },
                { "facilityTel", data?.Facility.Tel },
                { "reservationDateTime", formattedDate },
                { "code", rootCode },
                { "MainUserName", Reservation?.MainUser?.Name ?? Reservation?.Reserver?.Name },
                { "CheckInDate", $"{AppDate.GetDateTime(Reservation?.CheckInDate):yyyy/MM/dd}" },
                { "RestNumber", useDay ? 0 : Reservation?.RestNumber },
                { "CheckInTime", Reservation?.CheckInTime.To24HourFormat() },
                { "CheckOutDate", checkOutDate },
                { "CheckOutTime", (Reservation?.CheckOutTime ?? Reservation?.Plan?.CheckOut)?.ToHourMinuteString() },
                { "RoomNumber", Reservation?.RoomNumber },
                { "totalPersonDetail", TotalPersonsDetail(true) },
                { "RoomGroupName", data?.RoomGroup.Name },
                { "PlanName", data?.Plan.Name },
                { "reservationDetail", ReservationDetail(true) },
                { "payOff", GetPayOff() },
                {
                    "ManagementUrl", URL != null && Reservation?.Id != null
                        ? URL.Replace("{id}", $"{Reservation.Id}")
                        : null
                },
                { "applicationName", ApplicationName }
            };

            return Format.Body.FormatByName(args);
        }
    }

    /// <summary>
    /// 精算情報
    /// </summary>
    /// <returns></returns>
    private string GetPayOff()
    {
        var sb = new StringBuilder();
        var allTotalPrice = string.Empty;
        if (Reservation?.BookingData?.AllTotalPrice is not null)
        {
            allTotalPrice = ConvertUtil.ToString(Reservation.BookingData.AllTotalPrice, "#,###");
        }

        if (Reservation?.PaymentType == PaymentTypes.OnLinePayment)
        {
            sb.AppendLine("決済方法 : オンライン決済");
            sb.AppendLine($"合計金額(税込) : {allTotalPrice}円");
            sb.AppendLine($"オンライン決済：{allTotalPrice}円");
            sb.Append($"現地支払金額  ：{0}円");
        }
        else
        {
            sb.AppendLine("決済方法 : 現地決済");
            sb.AppendLine($"合計金額(税込) : {allTotalPrice}円");
            sb.Append($"現地支払金額  ：{allTotalPrice}円");
        }

        return sb.ToString();
    }

    public void SetData(
        Contexts.DataContexts.Entities.Data.Reservation reservation,
        Contexts.DataContexts.Entities.Data.Reservation oldReservation,
        string applicationName,
        ReservationOperationTypes reservationCancelType
    )
    {
        ApplicationName = applicationName;
        Reservation = reservation;
        OldReservation = oldReservation;
        ReservationOperationType = reservationCancelType;
    }

    public override void SetSample()
    {
        base.SetSample();

        ApplicationName = Settings.ApplicationNamePreview ?? "サンプルアプリ名";
        Reservation!.Facility!.Name =
            new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), Settings.FacilityNamePreview ?? "サンプル荘" } };
    }
}
