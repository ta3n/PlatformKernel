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

public class Io10003Template(
    Io10003TemplateFormat format,
    SmtpMailSetting smtpMailSetting
) : BaseTemplate
{
    private SmtpMailSetting Settings { get; set; } = smtpMailSetting;
    private Io10003TemplateFormat Format { get; set; } = format;
    protected override bool IsFacilityEmail => true;

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
            var reserver = Reservation?.Reserver;

            var mealTypes = string.Join(
                ",",
                Reservation?.Plan?.PlanMealTypes?.Select(
                    x => $" {x.MealType?.Name} ({GetMealTypeEatTypes(x.MealTypeEatType)} )"
                )
                ?? []
            );

            var formattedDate = DateTimeFormatWithOffset(
                Reservation?.ReservationDateTime ?? DateTime.UtcNow,
                data?.Facility?.TimeZone
            );

            var useDay = Reservation?.BookingData?.Plan.DayUse ?? false;
            var checkOutDate = useDay
                ? $"{AppDate.GetDateTime(Reservation?.CheckInDate):yyyy/MM/dd}"
                : $"{AppDate.GetDateTime(Reservation?.CheckInDate)?.AddDays(Reservation?.RestNumber ?? 0):yyyy/MM/dd}";

            var args = new FormatDictionary
            {
                { "facilityName", data?.Facility.Name },
                { "applicationName", ApplicationName },
                { "reservationDateTime", formattedDate },
                { "CheckInDate", $"{AppDate.GetDateTime(Reservation?.CheckInDate):yyyy/MM/dd}" },
                { "CheckInTime", Reservation?.CheckInTime.To24HourFormat() },
                { "RestNumber", useDay ? 0 : Reservation?.RestNumber },
                { "CheckOutDate", checkOutDate },
                { "CheckOutTime", (Reservation?.CheckOutTime ?? Reservation?.Plan?.CheckOut)?.ToHourMinuteString() },
                { "code", Reservation?.Code },
                { "RoomNumber", Reservation?.RoomNumber },
                { "totalPersonDetail", TotalPersonsDetail(true) },
                { "RoomGroupName", data?.RoomGroup.Name },
                { "PlanName", data?.Plan.Name },
                { "MealType", mealTypes },
                { "MainUserName", Reservation?.MainUser?.Name ?? Reservation?.Reserver?.Name },
                { "MainUserTel", Reservation?.MainUser?.Phone ?? Reservation?.Reserver?.Phone },
                { "ReserverName", Reservation?.Reserver?.Name },
                { "ReserverTel", Reservation?.Reserver?.Phone },
                { "questions", GetQuestions() },
                { "Memo", Reservation?.Memo },
                { "payOff", GetPayOff() },
                {
                    "url", URL != null && Reservation?.Id != null
                        ? URL.Replace("{id}", $"{Reservation.Id}")
                        : null
                },
                { "reservationDetail", ReservationDetail(true) },
                { MailParameter.ReserverNameKana.Key, reserver?.Kana },
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

    public override List<MailTemplateParameter> GetMailTemplateParameters()
    {
        return
        [
            ..BaseMailParameter.GetBaseMailBookingParameters(),
            MailParameter.MainUserTel,
            MailParameter.Questions,
            MailParameter.Memo,
            MailParameter.ManagerUrl
        ];
    }

    private static string GetMealTypeEatTypes(
        MealTypeEatTypes mealTypeEatTypes
    )
    {
        return mealTypeEatTypes switch
        {
            MealTypeEatTypes.Box => "個室",
            MealTypeEatTypes.Room => "部屋食",
            _ => "指定なし"
        };
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

    public override void SetSample()
    {
        base.SetSample();

        ApplicationName = Settings.ApplicationNamePreview ?? "サンプルアプリ名";
        Reservation!.Facility!.Name =
            new MultilingualText { { LanguageHeaderUtil.GetLanguageCodeFromHeader(), Settings.FacilityNamePreview ?? "サンプル荘" } };
    }
}
