using System.Text;
using Liberty.ApplicationShared.Domains.Services.Mails;
using Liberty.ApplicationShared.Extensions;
using Liberty.ApplicationShared.Utils;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Templates.FormatModels;

namespace Liberty.Reservation.Application.Templates;

public class Io10006EnTemplate(
    Io10006TemplateFormat format,
    SmtpMailService.SmtpMailSetting smtpMailSetting
) : Io10006Template(format, smtpMailSetting)
{
    public override string Body
    {
        get
        {
            var data = Reservation?.BookingData;
            var reserver = Reservation?.Reserver;

            var formattedDate = DateTimeFormatWithOffset(
                Reservation?.ReservationDateTime ?? DateTime.UtcNow,
                data?.Facility?.TimeZone
            );

            var useDay = Reservation?.BookingData?.Plan.DayUse ?? false;
            var checkOutDate = useDay
                ? $"{AppDate.GetDateTime(Reservation?.CheckInDate):yyyy/MM/dd}"
                : $"{AppDate.GetDateTime(Reservation?.CheckInDate)?.AddDays(Reservation?.RestNumber ?? 0):yyyy/MM/dd}";

            var rootCode = Reservation?.BookingData?.RootCode ?? string.Empty;
            var args = new FormatDictionary
            {
                { "ReserverName", Reservation?.Reserver?.Name },
                {
                    "managerCanceledDescription", ReservationOperationType == ReservationOperationTypes.Facility
                        ? "This reservation has been canceled by the facility."
                        : ""
                },
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
                { "cancellationInfo", CancellationInfo() },
                { "reservationDetail", ReservationDetail() },
                { "payOff", GetPayOff() },
                {
                    "UserUrl", URL != null && Reservation?.Id != null
                        ? URL.Replace("{id}", $"{Reservation.Id}")
                        : null
                },
                { "applicationName", ApplicationName },
                { MailParameter.ReserverNameKana.Key, reserver?.Kana },
                { MailParameter.ReserverTel.Key, reserver?.Phone },
                { MailParameter.ReserverEmail.Key, reserver?.EMail },
                { MailParameter.ReserverAddressCountry.Key, reserver?.Country?.Name },
                { MailParameter.ReserverAddressPostalCode.Key, reserver?.PostCode },
                { MailParameter.ReserverAddressPrefecture.Key, reserver?.Address1 },
                { MailParameter.ReserverAddressCity.Key, reserver?.Address2 },
                { MailParameter.ReserverAddressStreet.Key, reserver?.Address3 },
                { MailParameter.ReserverGender.Key, reserver?.Gender }
            };

            return Format.Body.FormatByName(args);
        }
    }

    /// <summary>
    /// 精算情報
    /// </summary>
    /// <returns></returns>
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
