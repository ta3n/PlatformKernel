using System.Text;
using Liberty.ApplicationShared.Domains.Services.Mails;
using Liberty.ApplicationShared.Extensions;
using Liberty.ApplicationShared.Utils;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Templates.FormatModels;

namespace Liberty.Reservation.Application.Templates;

public class Io10015EnTemplate(
    Io10015TemplateFormat format,
    SmtpMailService.SmtpMailSetting smtpMailSetting
) : Io10015Template(format, smtpMailSetting)
{
    public override string Body
    {
        get
        {
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
                { "ReserverName", Reservation?.Reserver?.Name },
                { "facilityName", data?.Facility.Name },
                { "facilityAddress", data?.Facility.FullAddress },
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
                { "totalPersonDetail", TotalPersonsDetail() },
                { "RoomGroupName", data?.RoomGroup.Name },
                { "PlanName", data?.Plan.Name },
                { "reservationDetail", ReservationDetail() },
                { "payOff", GetPayOff() },
                { "UserUrl", URL },
                { "applicationName", ApplicationName }
            };

            return Format.Body.FormatByName(args);
        }
    }

    protected override string GetPayOff()
    {
        var sb = new StringBuilder();
        var allTotalPrice = string.Empty;
        if (Reservation?.BookingData?.AllTotalPrice is not null)
        {
            allTotalPrice = ConvertUtil.ToString(Reservation.BookingData.AllTotalPrice, "#,###");
        }

        if (Reservation?.PaymentType == PaymentTypes.OnLinePayment)
        {
            sb.AppendLine("Payment Method : Online Payment");
            sb.AppendLine($"Total Amount (Including Tax) : {allTotalPrice}円");
            sb.AppendLine($"Online Payment：{allTotalPrice}円");
            sb.Append($"Amount to be Paid On-site  ：{0}円");
        }
        else
        {
            sb.AppendLine("Payment Method : Local Payment");
            sb.AppendLine($"Total Amount (Including Tax) : {allTotalPrice}円");
            sb.Append($"Amount to be Paid On-site  ：{allTotalPrice}円");
        }

        return sb.ToString();
    }
}
