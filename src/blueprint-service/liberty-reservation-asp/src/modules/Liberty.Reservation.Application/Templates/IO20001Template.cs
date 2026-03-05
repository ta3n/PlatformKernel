using System.Text;
using Liberty.ApplicationShared.Extensions;
using Liberty.ApplicationShared.Utils;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Templates.FormatModels;

namespace Liberty.Reservation.Application.Templates;

public class Io20001Template(
    Io20001TemplateFormat format
) : BaseTemplate
{
    private Io20001TemplateFormat Format { get; set; } = format;

    public override string Subject
    {
        get
        {
            var args = new FormatDictionary
            {
                { "applicationName", "リバティ プラン予約システム" },
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
                { "facilityName", data?.Facility.Name },
                { "applicationName", ApplicationName },
                { "reservationDateTime", formattedDate },
                { "CheckInDate", $"{AppDate.GetDateTime(Reservation?.CheckInDate):yyyy/MM/dd}" },
                { "CheckInTime", Reservation?.CheckInTime },
                { "RestNumber", Reservation?.RestNumber },
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
                        // 抽出条件がNG。idは施設によって値が異なる。
                        .Where(a => a.PricePeoples.Any(x => x.PersonAgeType.IsMain.HasValue && x.PersonAgeType.IsMain.Value))
                        .Select(a => a.MalePersons)
                        .Sum()
                    ?? 0
                },
                {
                    "1stRestFemaleNumber", data?.AppDates
                        ?.Where(a => a.AppDateId == Reservation?.CheckInDate)
                        .SelectMany(a => a.Rooms)
                        // 抽出条件がNG。idは施設によって値が異なる。
                        .Where(a => a.PricePeoples.Any(x => x.PersonAgeType.IsMain.HasValue && x.PersonAgeType.IsMain.Value))
                        .Select(a => a.FemalePersons)
                        .Sum()
                    ?? 0
                },
                {
                    "1stRestChildNumber", data?.AppDates
                        ?.Where(a => a.AppDateId == Reservation?.CheckInDate)
                        .SelectMany(a => a.Rooms)
                        // 抽出条件がNG。idは施設によって値が異なる。
                        .Where(a => a.PricePeoples.Any(x => x.PersonAgeType.IsMain.HasValue && !x.PersonAgeType.IsMain.Value))
                        .Select(a => a.Persons)
                        .Sum()
                    ?? 0
                },
                { "RoomGroupName", data?.RoomGroup.Name },
                { "PlanName", data?.Plan.Name },
                { "MealType", "" },
                { "MainUserName", Reservation?.MainUser?.Name },
                { "MainUserTel", Reservation?.MainUser?.Phone },
                { "ReserverName", Reservation?.Reserver?.Name },
                { "ReserverTel", Reservation?.Reserver?.Phone },
                { "questions", GetQuestions() },
                { "Memo", Reservation?.Memo },
                { "payOff", GetPayOff() },
                { "url", $"{Reservation?.Facility?.Url}/y017" },
                { "reservationDetail", ReservationDetail() }
            };

            return Format.Body.FormatByName(args);
        }
    }

    /// <summary>
    /// 質問項目
    /// </summary>
    /// <returns></returns>
    private string GetQuestions()
    {
        var sb = new StringBuilder();

        var questions = Reservation?.BookingData?.PlanQuestions ?? [];

        var qIndex = 1;
        foreach (var q in questions)
        {
            sb.AppendLine($"質問{qIndex}       ：  {q.Name}");
            sb.AppendLine($"回答{qIndex}       ：  {q.AnswerData}");
            qIndex++;
        }

        return sb.ToString();
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
