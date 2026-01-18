using System.Globalization;
using System.Text;
using Liberty.ApplicationShared.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

namespace Liberty.Reservation.Application.Templates;

public interface IIoTemplate
{
    string Subject { get; }
    string Body { get; }

    void SetSample();
}

public abstract class BaseTemplate : IIoTemplate
{
    public abstract string Subject { get; }
    public abstract string Body { get; }
    protected virtual bool IsFacilityEmail => false;

    public virtual string FromDisplayName
    {
        get
        {
            var data = Reservation?.BookingData;
            var languageCode = IsFacilityEmail
                ? DefaultValues.LanguageCode
                : data?.LanguageCode ?? DefaultValues.LanguageCode;

            var facilityName = data?.Facility?.LocalizedNames?.GetValueByCode(languageCode) ?? data?.Facility?.Name;
            var siteName = data?.Site?.LocalizedNames?.GetValueByCode(languageCode) ?? data?.Site?.Name;

            return FormatFromDisplayName(languageCode, facilityName, siteName);
        }
    }

    protected virtual string? ApplicationName { get; set; }
    protected virtual Contexts.DataContexts.Entities.Data.Reservation? Reservation { get; set; }
    public string? URL { get; set; }

    public override bool Equals(
        object? obj
    )
    {
        return obj is BaseTemplate template && Subject == template.Subject && Body == template.Body;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Subject, Body);
    }

    public virtual void SetSample()
    {
        var reservation = SetReservation();

        ApplicationName = "リバティプラン予約システム";
        Reservation = reservation;
    }

    public virtual List<MailTemplateParameter> GetMailTemplateParameters()
    {
        return [];
    }

    private static string FormatFromDisplayName(
        string languageCode,
        string? facilityName,
        string? siteName
    )
    {
        return languageCode switch
        {
            DefaultValues.LanguageCode => $"【{facilityName}】- {siteName}予約システム通知",
            _ => $"【{facilityName}】- {siteName} Reservation System Notification"
        };
    }

    private static Cancellation GetCancellationDetail()
    {
        var cancellationData = new CancellationData
        {
            DayStart = 1,
            DayEnd = 5,
            Rate = 10,
            Description = new MultilingualText { { "ja", "10% charge for cancellations within 5 days" } }
        };

        var cancellationCancellationData = new CancellationCancellationData
        {
            CancellationId = 1,
            CancellationDataId = 1,
            CancellationData = cancellationData
        };

        var cancellation = new Cancellation
        {
            Name = new MultilingualText { { "ja", "Cancel Name" } },
            CancellationCancellationDatas = [cancellationCancellationData]
        };

        return cancellation;
    }

    /// <summary>
    /// 予約明細
    /// </summary>
    /// <returns></returns>
    protected string ReservationDetail(
        bool? isFacilityEmail = false
    )
    {
        var sb = new StringBuilder();

        var appDates = Reservation?.BookingData?.AppDates;
        var languageCode = Reservation?.BookingData?.LanguageCode;

        if (isFacilityEmail is true)
        {
            languageCode = DefaultValues.LanguageCode;
        }

        var appDateIds = appDates?
                .Select(a => a.AppDateId)
                .Distinct()
                .OrderBy(a => a)
                .ToArray()
            ?? [];

        var restNumber = Reservation?.RestNumber ?? 0;
        var roomNumber = Reservation?.RoomNumber ?? 0;

        for (var restIndex = 0; restIndex < restNumber; restIndex++)
        {
            AppendNightHeader(sb, restIndex, appDateIds, languageCode);

            var appDate = appDates?.SingleOrDefault(a => a.AppDateId == appDateIds[restIndex]);
            AppendRoomDetails(sb, roomNumber, restIndex, restNumber, appDate, languageCode);
        }

        return sb.ToString();
    }

    protected string TotalPersonsDetail(
        bool? isFacilityEmail = false
    )
    {
        var languageCode = Reservation?.BookingData?.LanguageCode;

        if (isFacilityEmail is true)
        {
            languageCode = DefaultValues.LanguageCode;
        }

        var isInconsistent = Reservation?.BookingData?.AppDates?
                .Select(
                    appDate =>
                    {
                        var roomStructures = appDate.Rooms
                            .Select(
                                room =>
                                {
                                    var pricePeoples = room.PricePeoples;

                                    var structureKeys = pricePeoples
                                        .Select(
                                            p => new
                                            {
                                                Male = p.MalePersons ?? 0,
                                                Female = p.FemalePersons ?? 0,
                                                None = p.NonePersons ?? 0,
                                                Other = p.OtherPersons ?? 0
                                            }
                                        )
                                        .Select(x => $"{x.Male}-{x.Female}-{x.None}-{x.Other}")
                                        .Order();

                                    return string.Join("|", structureKeys);
                                }
                            )
                            .Order();

                        return string.Join("||", roomStructures);
                    }
                )
                .Distinct()
                .Count()
            > 1;

        if (isInconsistent)
        {
            return languageCode switch
            {
                DefaultValues.LanguageEnCode =>
                    "Total number of guests: The number of guests may vary depending on the stay date, so please check the breakdown.",
                _ => "合計人数：宿泊日により人数が異なるため、人数内訳をご確認ください"
            };
        }

        var rooms = (Reservation?.BookingData?.AppDates?.FirstOrDefault()?.Rooms ?? []).ToList();

        var maleNumber = rooms.Sum(p => p.MalePersons) ?? 0;
        var femaleNumber = rooms.Sum(p => p.FemalePersons) ?? 0;
        var totalPersons = rooms.Sum(p => p.Persons);
        var childNumber = totalPersons - (maleNumber + femaleNumber);

        var sb = new StringBuilder();

        if (languageCode == DefaultValues.LanguageEnCode)
        {
            sb.AppendLine($"Total Guests: {totalPersons}");
            sb.AppendLine($"Adults: {maleNumber} male(s), {femaleNumber} female(s)");
            sb.AppendLine($"Children/Infants: {childNumber}");
        }
        else
        {
            sb.AppendLine($"合計人数： {totalPersons}名");
            sb.AppendLine($"大人： 男性({maleNumber})名 女性({femaleNumber})名");
            sb.AppendLine($"子供・幼児： {childNumber}名");
        }

        return sb.ToString();
    }

    private static void AppendNightHeader(
        StringBuilder sb,
        int restIndex,
        long[] appDateIds,
        string? languageCode = DefaultValues.LanguageCode
    )
    {
        var date = AppDate.GetDateTime(appDateIds[restIndex]);
        var ordinal = GetOrdinalSuffix(restIndex + 1);
        var header = languageCode switch
        {
            DefaultValues.LanguageCode => $"・{restIndex + 1}泊目  :  {restIndex + 1}泊目日付({date:yyyy/MM/dd})",
            DefaultValues.LanguageEnCode =>
                $"・{restIndex + 1}{ordinal} Night  : Date of {restIndex + 1}{ordinal} Night ({date:yyyy/MM/dd})",
            _ => $"・{restIndex + 1}泊目  :  {restIndex + 1}泊目日付({date:yyyy/MM/dd})"
        };

        sb.AppendLine(header);
    }

    private static void AppendRoomDetails(
        StringBuilder sb,
        int roomGroupCount,
        int restIndex,
        int restNumber,
        BookingAppDateData? appDate,
        string? languageCode = DefaultValues.LanguageCode
    )
    {
        for (var roomGroupIndex = 0; roomGroupIndex < roomGroupCount; roomGroupIndex++)
        {
            var ordinal = GetOrdinalSuffix(roomGroupIndex + 1);
            var roomHeader = languageCode switch
            {
                DefaultValues.LanguageCode => $"  部屋 ({roomGroupIndex + 1}部屋目)",
                DefaultValues.LanguageEnCode => $"  Room ({roomGroupIndex + 1}{ordinal} Room)",
                _ => $"  部屋 ({roomGroupIndex + 1}部屋目)"
            };

            sb.AppendLine(roomHeader);

            // 人数料金
            AppendPricePeopleDetails(sb, appDate, roomGroupIndex, languageCode);

            // オプション料金
            AppendOptionItemDetails(sb, appDate, roomGroupIndex, languageCode);

            // 空白行 (最終行は空白行不要)
            if (!(restIndex == restNumber - 1 && roomGroupIndex == roomGroupCount - 1))
            {
                sb.AppendLine();
            }
        }
    }

    private static void AppendPricePeopleDetails(
        StringBuilder sb,
        BookingAppDateData? appDate,
        int roomGroupIndex,
        string? languageCode = DefaultValues.LanguageCode
    )
    {
        var pricePeoples = appDate?.Rooms
                .Where(x => x.RoomIndex == roomGroupIndex)
                .SelectMany(x => x.PricePeoples)
            ?? [];
        var currencyDisplayName = languageCode switch
        {
            DefaultValues.LanguageCode => "円",
            DefaultValues.LanguageEnCode => " yen",
            _ => "円"
        };

        var personDisplayName = languageCode switch
        {
            DefaultValues.LanguageCode => "名",
            DefaultValues.LanguageEnCode => " persons",
            _ => "名"
        };

        foreach (var d in pricePeoples)
        {
            var name = d.PersonAgeType.Name ?? "大人";
            var price = d.RoomPrice;
            var displayPrice = price == 0 ? "0" : ConvertUtil.ToString(price, "#,###");
            var persons = d.Persons;
            var text = $"    {name}  :  {displayPrice}{currencyDisplayName}  ×  {persons}{personDisplayName}";
            sb.AppendLine(text);
        }
    }

    private static void AppendOptionItemDetails(
        StringBuilder sb,
        BookingAppDateData? appDate,
        int roomGroupIndex,
        string? languageCode = DefaultValues.LanguageCode
    )
    {
        var currencyDisplayName = languageCode switch
        {
            DefaultValues.LanguageCode => "円",
            DefaultValues.LanguageEnCode => " yen",
            _ => "円"
        };

        var optionItems = appDate?.Rooms
                .Where(x => x.RoomIndex == roomGroupIndex)
                .SelectMany(x => x.OptionItems!)
            ?? [];

        foreach (var d in optionItems)
        {
            var name = d.Name;
            var price = d.Price ?? 0;
            var displayPrice = ConvertUtil.ToString(price, "#,###");
            var number = d.Number;
            var text = $"    {name}  :  {displayPrice}{currencyDisplayName}  ×  {number}";
            sb.AppendLine(text);
        }
    }

    private static Contexts.DataContexts.Entities.Data.Reservation SetReservation()
    {
        var reservation = new Contexts.DataContexts.Entities.Data.Reservation
        {
            Code = "112233",
            CheckInDate = 20200801,
            CheckInTime = new TimeSpan(18, 0, 0),
            CheckOutTime = new TimeSpan(10, 0, 0),
            RestNumber = 2,
            RoomNumber = 3,
            Reserver = new(),
            MainUser = new(),
            Plan = new()
        };

        // 予約時以降に宿が修正を許容するため
        var facility = new Facility
        {
            Phone = "xxxx-xx-xxxx",
            Url = "https://xxxx.xx"
        };

        var reservationData = new BookingData
        {
            Facility = new FacilityData(),
            Plan = new PlanData(),
            RoomGroup = new RoomGroupData(),
            Site = new SiteData()
        };

        reservationData.Facility.Name = "サンプル旅館";

        reservation.Reserver.Address1 = "静岡県";
        reservation.Reserver.Address2 = "静岡市駿河区稲川";
        reservation.Reserver.Address3 = "3丁目2番23号";
        reservation.Reserver.Gender = 0;
        reservation.Reserver.Phone = "00011113333";
        reservation.Reserver.PostCode = "422-8062";
        reservation.Reserver.Name = "ユーザ";
        reservation.Reserver.Kana = "ゆーざ";

        reservation.MainUser.Address1 = "静岡県";
        reservation.MainUser.Address2 = "静岡市駿河区稲川";
        reservation.MainUser.Address3 = "3丁目2番23号";
        reservation.MainUser.Gender = 0;
        reservation.MainUser.Phone = "00022224444";
        reservation.MainUser.PostCode = "422-8062";
        reservation.MainUser.Name = "メインユーザ";
        reservation.MainUser.Kana = "めいんゆーざ";

        reservationData.Plan.Name = "海に浮かぶ富士を望む露天風呂付";
        reservationData.RoomGroup.Name = "露天風呂付";

        // 予約明細（宿泊ｘ部屋）
        var appDateDataOfReservation = new List<BookingAppDateData>();

        for (var restIndex = 0; restIndex < reservation.RestNumber; restIndex++)
        {
            var appDateId = AppDate.GetId(AppDate.GetDateTime(reservation.CheckInDate).AddDays(restIndex));
            var rooms = new List<BookingRoomDataOfAppDate>();

            for (var roomGroupIndex = 0; roomGroupIndex < reservation.RoomNumber; roomGroupIndex++)
            {
                var pricePeoples = new List<PeoplePriceDataOfRoom>();
                var optionItems = new List<OptionItemDataOfRoom>();

                // 大人
                pricePeoples.Add(
                    BuildReservationRoomGroupAppDatePersonAgeTypeData(
                        restIndex,
                        1,
                        "大人",
                        14000,
                        2,
                        1
                    )
                );

                // 子供
                pricePeoples.Add(
                    BuildReservationRoomGroupAppDatePersonAgeTypeData(
                        restIndex,
                        2,
                        "子供",
                        7000,
                        2,
                        0
                    )
                );

                // 幼児(食事・布団)
                pricePeoples.Add(
                    BuildReservationRoomGroupAppDatePersonAgeTypeData(
                        restIndex,
                        3,
                        "幼児(食事・布団)",
                        4000,
                        4,
                        2
                    )
                );

                // 幼児(食事)
                pricePeoples.Add(
                    BuildReservationRoomGroupAppDatePersonAgeTypeData(
                        restIndex,
                        4,
                        "幼児(食事)",
                        3000,
                        4,
                        1
                    )
                );

                // 幼児(布団)
                pricePeoples.Add(
                    BuildReservationRoomGroupAppDatePersonAgeTypeData(
                        restIndex,
                        5,
                        "幼児(布団)",
                        2000,
                        3,
                        2
                    )
                );

                // 幼児
                pricePeoples.Add(
                    BuildReservationRoomGroupAppDatePersonAgeTypeData(
                        restIndex,
                        6,
                        "幼児",
                        1000,
                        2,
                        1
                    )
                );

                // オプション
                for (var optionId = 0; optionId < 5; optionId++)
                {
                    var number = restIndex + roomGroupIndex + 1;

                    var optionItem = new OptionItemDataOfRoom
                    {
                        Id = optionId,
                        Name = "オプション" + optionId,
                        Price = 1000 + optionId,
                        Number = number
                    };
                    optionItems.Add(optionItem);
                }

                rooms.Add(
                    new()
                    {
                        PricePeoples = pricePeoples,
                        OptionItems = optionItems
                    }
                );
            }

            var appDateData = new BookingAppDateData
            {
                AppDateId = appDateId,
                Rooms = rooms
            };

            appDateDataOfReservation.Add(appDateData);
            reservationData.AppDates = appDateDataOfReservation;
        }

        reservation.Facility = facility;
        reservation.BookingData = reservationData;
        reservation.Plan!.Cancellation = GetCancellationDetail();
        return reservation;
    }

    private static PeoplePriceDataOfRoom BuildReservationRoomGroupAppDatePersonAgeTypeData(
        int restIndex,
        int id,
        string name,
        int price,
        int number,
        int maleNumber
    )
    {
        var reservationPersonAgeTypeData = new PeopleDataOfRoom
        {
            Id = id,
            Name = name,
            AgeMax = null,
            AgeMin = null
        };

        if (restIndex == 0)
        {
            price = price * 4 / 5;
            number = 2;
            maleNumber = 1;
        }

        var femaleNumber = number - maleNumber;
        const int spaTax = 150;

        var reservationRoomGroupAppDatePersonAgeTypeData = new PeoplePriceDataOfRoom
        {
            PersonAgeType = reservationPersonAgeTypeData,
            Persons = number,
            MalePersons = maleNumber,
            FemalePersons = femaleNumber,
            NonePersons = 0,
            RoomPrice = price,
            SpaTax = spaTax
        };

        return reservationRoomGroupAppDatePersonAgeTypeData;
    }

    protected static string DateTimeFormatWithOffset(
        DateTime dateTime,
        string? timezoneString
    )
    {
        var defaultOffset = TimeSpan.FromHours(9);
        TimeSpan offset;

        if (!string.IsNullOrWhiteSpace(timezoneString))
        {
            if (!TimeSpan.TryParse(
                    timezoneString,
                    CultureInfo.InvariantCulture,
                    out offset
                ))
            {
                offset = defaultOffset;
            }
        }
        else
        {
            offset = defaultOffset;
        }

        var dateTimeOffset = new DateTimeOffset(dateTime, TimeSpan.Zero).ToOffset(offset);
        return dateTimeOffset.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
    }

    private static string GetOrdinalSuffix(
        int number
    )
    {
        var lastTwoDigits = number % 100;
        var lastDigit = number % 10;

        if (lastTwoDigits is >= 11 and <= 13)
        {
            return "th";
        }

        return lastDigit switch
        {
            1 => "st",
            2 => "nd",
            3 => "rd",
            _ => "th"
        };
    }

    protected string CancellationInfo(
        bool? isFacilityEmail = false
    )
    {
        var sb = new StringBuilder();

        var languageCode = Reservation?.BookingData?.LanguageCode;

        if (isFacilityEmail is true)
        {
            languageCode = DefaultValues.LanguageCode;
        }

        var content = languageCode switch
        {
            DefaultValues.LanguageCode => GetCancellationInfoJaDetail(),
            DefaultValues.LanguageEnCode => GetCancellationInfoEnDetail(),
            _ => string.Empty
        };
        sb.Append(content);
        return sb.ToString();
    }

    private string GetCancellationInfoEnDetail()
    {
        var sb = new StringBuilder();
        var paymentInfo = Reservation?.PaymentType;
        var offset = GetFacilityTimeZoneOffset();

        sb.AppendLine($"Cancellation Date: {Reservation?.CancelledDateTime?.Add(offset):yyyy/MM/dd HH:mm}");
        sb.AppendLine($"Cancellation Fee: Applicable Rate: {Reservation?.CancelRateFee}%");
        sb.AppendLine($"Cancellation Amount: {Reservation?.CancellationPrice} yen");

        var payment = paymentInfo switch
        {
            PaymentTypes.OnLinePayment => "Online Payment",
            PaymentTypes.OnSidePayment => "Bank transfer (Please check the information regarding cancellation fees)",
            _ => string.Empty
        };
        sb.AppendLine($"Cancellation Fee Collection Method: {payment}");
        sb.AppendLine("Information regarding cancellation fees: ");
        sb.AppendLine(Reservation?.BookingData?.Plan.CancellationDataPolicy?.RuleDetail?.GetValueByCode(DefaultValues.LanguageEnCode));
        return sb.ToString();
    }

    private string GetCancellationInfoJaDetail()
    {
        var sb = new StringBuilder();
        var paymentInfo = Reservation?.PaymentType;
        var offset = GetFacilityTimeZoneOffset();
        sb.AppendLine($"キャンセル処理日: {Reservation?.CancelledDateTime?.Add(offset).ToString("yyyy年MM月dd日 HH時mm分")}");
        sb.AppendLine($"キャンセル料：該当するキャンセル料率: {Reservation?.CancelRateFee}%");
        sb.AppendLine($"キャンセル金額: {Reservation?.CancellationPrice}円");

        var payment = paymentInfo switch
        {
            PaymentTypes.OnLinePayment => "オンライン決済",
            PaymentTypes.OnSidePayment => "振込（キャンセル料に関するご案内をご確認ください）",
            _ => string.Empty
        };
        sb.AppendLine($"キャンセル料徴収方法：{payment}");
        sb.AppendLine("キャンセル料に関するご案内: ");
        sb.AppendLine(Reservation?.BookingData?.Plan.CancellationDataPolicy?.RuleDetail?.GetValueByCode(DefaultValues.LanguageCode));
        return sb.ToString();
    }

    protected TimeSpan GetFacilityTimeZoneOffset()
    {
        var defaultOffset = DefaultValues.DefaultTimeZoneOffset;

        var timeZoneString = Reservation?
            .BookingData?
            .Facility?
            .TimeZone;

        return TimeSpan.TryParse(
            timeZoneString,
            CultureInfo.InvariantCulture,
            out var offset
        )
            ? offset
            : defaultOffset;
    }
}
