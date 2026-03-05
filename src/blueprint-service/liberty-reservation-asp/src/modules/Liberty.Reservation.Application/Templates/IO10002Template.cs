using System.Text;
using Liberty.ApplicationShared.Extensions;
using Liberty.ApplicationShared.Utils;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Templates.FormatModels;

namespace Liberty.Reservation.Application.Templates;

public class Io10002Template(
    Io10002TemplateFormat format
) : BaseTemplate
{
    private Io10002TemplateFormat Format { get; set; } = format;

    public override string Subject
    {
        get
        {
            var args = new FormatDictionary
            {
                { "applicationName", ApplicationName },
                { "code", Reservation?.Code }
            };
            return Format.Subject.FormatByName(args);
        }
    }

    public override string Body
    {
        get
        {
            var data = Reservation?.BookingData;
            var formattedDate = DateTimeFormatWithOffset(
                Reservation?.ReservationDateTime ?? DateTime.UtcNow,
                Reservation?.BookingData?.TimeZoneOffset ?? 0
            );

            var args = new FormatDictionary
            {
                { "ReserverName", Reservation?.Reserver?.Name },
                { "facilityName", data?.Facility.Name },
                { "OnlineSettlementUrl", Url },
                { "facilityAddress", data?.Facility.Address },
                { "facilityTel", data?.Facility.Tel },
                { "reservationDateTime", formattedDate },
                { "code", Reservation?.Code },
                { "MainUserName", Reservation?.MainUser?.Name },
                { "CheckInDate", $"{AppDate.GetDateTime(Reservation?.CheckInDate):yyyy/MM/dd}" },
                { "RestNumber", Reservation?.RestNumber },
                { "CheckInTime", Reservation?.CheckInTime.To24HourFormat() },
                { "CheckOutDate", $"{AppDate.GetDateTime(Reservation?.CheckInDate)?.AddDays(Reservation?.RestNumber ?? 0):yyyy/MM/dd}" },
                { "CheckOutTime", Reservation?.CheckOutTime?.ToHourMinuteString() },
                { "RoomNumber", Reservation?.RoomNumber },
                {
                    "TotalPersons", data?.AppDates
                        ?.Where(a => a.AppDateId == Reservation?.CheckInDate)
                        .SelectMany(a => a.Rooms)
                        .Select(a => a.Persons)
                        .Sum()
                    ?? 0
                },
                {
                    "1stRestMaleNumber", data?.AppDates
                        ?.Where(a => a.AppDateId == Reservation?.CheckInDate)
                        .SelectMany(a => a.Rooms)
                        .Where(a => a.PricePeoples.Any(x => x.PersonAgeType.IsMain.HasValue && x.PersonAgeType.IsMain.Value))
                        .Select(a => a.MalePersons)
                        .Sum()
                    ?? 0
                },
                {
                    "1stRestFemaleNumber", data?.AppDates
                        ?.Where(a => a.AppDateId == Reservation?.CheckInDate)
                        .SelectMany(a => a.Rooms)
                        .Where(a => a.PricePeoples.Any(x => x.PersonAgeType.IsMain.HasValue && x.PersonAgeType.IsMain.Value))
                        .Select(a => a.FemalePersons)
                        .Sum()
                    ?? 0
                },
                {
                    "1stRestChildNumber", data?.AppDates
                        ?.Where(a => a.AppDateId == Reservation?.CheckInDate)
                        .SelectMany(a => a.Rooms)
                        .Where(a => a.PricePeoples.Any(x => x.PersonAgeType.IsMain.HasValue && !x.PersonAgeType.IsMain.Value))
                        .Select(a => a.Persons)
                        .Sum()
                    ?? 0
                },
                { "RoomGroupName", data?.RoomGroup.Name },
                { "PlanName", data?.Plan.Name },
                { "reservationDetail", ReservationDetail() },
                { "payOff", GetPayOff() },
                { "applicationName", ApplicationName },
                { "userUrl", GetUserUrl() }
            };

            return Format.Body.FormatByName(args);
        }
    }

    private string? GetUserUrl()
    {
        if (URL != null && (Reservation?.MainUserId != null || Reservation?.ReserverId != null))
        {
            return Reservation.IsSameMainUser
                ? URL.Replace("{id}", $"{Reservation?.MainUserId}")
                : URL.Replace("{id}", $"{Reservation?.ReserverId}");
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// オンライン決済URL
    /// </summary>
    private string Url
    {
        get
        {
            var args = new FormatDictionary();

            return Format.Url.FormatByName(args);
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
        string applicationName
    )
    {
        ApplicationName = applicationName;
        Reservation = reservation;
    }
}
