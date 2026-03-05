using System.Text;
using Liberty.ApplicationShared.Domains.Services.Mails;
using Liberty.ApplicationShared.Extensions;
using Liberty.ApplicationShared.Utils;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Templates.FormatModels;

namespace Liberty.Reservation.Application.Templates;

public class Io10101EnTemplate(
    Io10101TemplateFormat format,
    SmtpMailService.SmtpMailSetting smtpMailSetting
) : Io10101Template(format, smtpMailSetting)
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
            var cancelPeriod = data?.Plan.CancellationDataPolicy?.CancellationData != null
                && data.Plan.CancellationDataPolicy.CancellationData.Any()
                    ? data.Plan.CancellationDataPolicy.CancellationData.Max(x => x.DayEnd)
                    : 0;

            var dayLeft = AppDate.GetDateTime(Reservation?.CheckInDate)?.Date - DateTime.UtcNow.AddHours(DefaultValues.TimeZoneOffset).Date;

            var args = new FormatDictionary
            {
                { "ReserverName", Reservation?.Reserver?.Name },
                { "dayLeft", dayLeft?.TotalDays ?? 0 }, //ハードコーディング
                { "facilityName", data?.Facility.Name },
                { "cancelPeriod", cancelPeriod }, //ハードコーディング
                { "facilityAddress", data?.Facility.FullAddress },
                { "facilityTel", data?.Facility.Tel },
                { "reservationDateTime", formattedDate },
                { "code", Reservation?.Code },
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
                {
                    "UserUrl", URL != null && Reservation?.Id != null
                        ? URL.Replace("{id}", $"{Reservation.Id}")
                        : null
                },
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
