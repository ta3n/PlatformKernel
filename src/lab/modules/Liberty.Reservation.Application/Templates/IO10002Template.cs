using System.Text;
using Liberty.ApplicationShared.Extensions;
using Liberty.ApplicationShared.Utils;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;

namespace Liberty.Reservation.Application.Templates;

public class IO10002Template(
    IO10002TemplateFormat format
) : BaseTemplate
{
    public IO10002TemplateFormat Format { get; private set; } = format;

    private string? ApplicationName { get; set; }
    private Contexts.DataContexts.Entities.Data.Reservation? Reservation { get; set; }

    public override string Subject
    {
        get
        {
            var args = new FormatDictionary
            {
                { "applicationName", ApplicationName },
                { "code", Reservation?.ReservationData?.Code }
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
                { "ReserverName", data?.Reserver?.Name },
                { "facilityName", data?.Facility?.Name },
                { "OnlineSettlementUrl", Url },
                { "facilityAddress", Reservation?.Facility?.Address },
                { "facilityTel", Reservation?.Facility?.Phone },
                { "reservationDateTime", $"{Reservation?.ReservationDateTime:yyyy年MM月dd日 HH:mm:ss}" },
                { "code", Reservation?.ReservationData?.Code },
                { "MainUserName", data?.MainUser?.Name },
                { "CheckInDate", $"{AppDate.GetDateTime(data?.CheckInDate):yyyy/MM/dd}" },
                { "RestNumber", data?.RestNumber },
                { "CheckInTime", data?.CheckInTime },
                {
                    "CheckOutDate",
                    $"{AppDate.GetDateTime(data?.CheckInDate)?.AddDays(data?.RestNumber ?? 0):yyyy/MM/dd}"
                },
                { "CheckOutTime", data?.CheckOutTime },
                { "RoomNumber", data?.RoomNumber },
                {
                    "TotalPersons", data?.ReservationPlanRoomGroupAppDates
                        ?.Where(a => a.AppDateId == data.CheckInDate)
                        .SelectMany(a => a.ReservationRoomGroupAppDatePersonAgeTypes ?? [])
                        .Select(a => a.Persons)
                        .Sum() ?? 0
                },
                {
                    "1stRestMaleNumber", data?.ReservationPlanRoomGroupAppDates
                        ?.Where(a => a.AppDateId == data.CheckInDate)
                        .SelectMany(a => a.ReservationRoomGroupAppDatePersonAgeTypes ?? [])
                        // FIXME: 抽出条件がNG。idは施設によって値が異なる。
                        .Where(a => a.PersonAgeType?.Id == 1)
                        .Select(a => a.MalePersons)
                        .Sum() ?? 0
                },
                {
                    "1stRestFemaleNumber", data?.ReservationPlanRoomGroupAppDates
                        ?.Where(a => a.AppDateId == data.CheckInDate)
                        .SelectMany(a => a.ReservationRoomGroupAppDatePersonAgeTypes ?? [])
                        // FIXME: 抽出条件がNG。idは施設によって値が異なる。
                        .Where(a => a.PersonAgeType?.Id == 1)
                        .Select(a => a.FemalePersons)
                        .Sum() ?? 0
                },
                {
                    "1stRestChildNumber", data?.ReservationPlanRoomGroupAppDates
                        ?.Where(a => a.AppDateId == data.CheckInDate)
                        .SelectMany(a => a.ReservationRoomGroupAppDatePersonAgeTypes ?? [])
                        // FIXME: 抽出条件がNG。idは施設によって値が異なる。
                        .Where(a => a.PersonAgeType?.Id != 1)
                        .Select(a => a.Persons)
                        .Sum() ?? 0
                },
                { "RoomGroupName", data?.RoomGroup?.Name },
                { "PlanName", data?.Plan?.Name },
                { "reservationDetail", ReservationDetail() },
                { "payOff", GetPayOff() },
                { "facilityAddressGoogleMap", "" }, //fixme:ハードコーディング

                { "applicationName", ApplicationName },
                { "userUrl", "" } //fixme:ハードコーディング
            };

            return Format.Body.FormatByName(args);
        }
    }

    /// <summary>
    /// オンライン決済URL
    /// </summary>
    public string Url
    {
        get
        {
            // FIXME:項目確認中
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
        if (Reservation?.ReservationData?.AllTotalPrice is not null)
        {
            allTotalPrice = ConvertUtil.ToString(Reservation.ReservationData.AllTotalPrice, "#,###");
        }

        if (Reservation?.ReservationData?.PaymentType == PaymentTypes.OnLinePayment.ToString())
        {
            sb.AppendLine($"決済方法 : オンライン決済");
            sb.AppendLine($"合計金額(税込) : {allTotalPrice}円");
            sb.AppendLine($"オンライン決済：{allTotalPrice}円");
            sb.Append($"現地支払金額  ：{0}円");
        }
        else
        {
            sb.AppendLine($"決済方法 : 現地決済");
            sb.AppendLine($"合計金額(税込) : {allTotalPrice}円");
            sb.Append($"現地支払金額  ：{allTotalPrice}円");
        }

        return sb.ToString();
    }

    /// <summary>
    /// 予約明細
    /// </summary>
    /// <returns></returns>
    public string ReservationDetail()
    {
        var sb = new StringBuilder();

        var datas = Reservation?.ReservationData?.ReservationPlanRoomGroupAppDates;
        var appDateIds = datas?
            .Select(a => a.AppDateId)
            .Distinct()
            .OrderBy(a => a)
            .ToArray() ?? [];

        var restNumber = Reservation?.ReservationData?.RestNumber ?? 0;
        var roomNumber = Reservation?.ReservationData?.RoomNumber ?? 0;

        for (var restIndex = 0; restIndex < restNumber; restIndex++)
        {
            {
                var text =
                    $"・{restIndex + 1}泊目  :  {restIndex + 1}泊目日付({AppDate.GetDateTime(appDateIds[restIndex]):yyyy/MM/dd})";
                sb.AppendLine(text);
            }

            for (var roomGroupIndex = 0; roomGroupIndex < roomNumber; roomGroupIndex++)
            {
                var textIndex = $"  部屋 ({roomGroupIndex + 1}部屋目)";
                sb.AppendLine(textIndex);

                var data = datas?
                    .Where(a => a.AppDateId == appDateIds[restIndex])
                    .SingleOrDefault(a => a.RoomGroupIndex == roomGroupIndex);

                // 人数料金
                foreach (var d in data?.ReservationRoomGroupAppDatePersonAgeTypes ?? [])
                {
                    var personAgeType = d.PersonAgeType;
                    var name = d.PersonAgeType?.Name;
                    var price = d.Price;
                    var spaTax = d.SpaTax;
                    var displayPrice = ConvertUtil.ToString(price + spaTax, "#,###");
                    var persons = d.Persons;

                    var text = $"    {name}  :  {displayPrice}円  ×  {persons}名";
                    sb.AppendLine(text);
                }

                // オプション料金
                foreach (var d in data?.ReservationRoomAppDateOptionItems ?? [])
                {
                    var optionItem = d.OptionItem;
                    var name = d.OptionItem?.Name;
                    var price = d.Price;
                    var displayPrice = ConvertUtil.ToString(price, "#,###");
                    var number = d.Number;

                    var text = $"    {name}  :  {displayPrice}円  ×  {number}";
                    sb.AppendLine(text);
                }

                // 空白行 (最終行は空白行不要)
                if (restIndex + 1 == restNumber && roomGroupIndex + 1 == roomNumber)
                {
                    continue;
                }

                sb.AppendLine();
            }
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
        var reservation = new Contexts.DataContexts.Entities.Data.Reservation();

        var applicationName = "リバティプラン予約システム";

        // 予約時以降に宿が修正を許容するため
        var facility = new Facility
        {
            Address1 = "施設住所1",
            Address2 = "施設住所2",
            Address3 = "施設住所3",
            Phone = "xxxx-xx-xxxx",
            Url = "https://xxxx.xx"
        };

        var reservationData = new ReservationData
        {
            Facility = new ReservationFacilityData(),
            MainUser = new ReservationUserData(),
            Reserver = new ReservationUserData(),
            Plan = new ReservationPlanData(),
            RoomGroup = new ReservationRoomGroupData(),
            Code = "112233",
            CheckInDate = 20200801,
            CheckInTime = "18:00:00",
            CheckOutTime = "10:00:00",
            RestNumber = 2,
            RoomNumber = 3
        };

        reservationData.Facility.Name = "サンプル旅館";

        reservationData.Reserver.Address1 = "静岡県";
        reservationData.Reserver.Address2 = "静岡市駿河区稲川";
        reservationData.Reserver.Address3 = "3丁目2番23号";
        reservationData.Reserver.AppDateId = reservationData.CheckInDate;
        reservationData.Reserver.Birthday = 19910101;
        reservationData.Reserver.Gender = "0";
        reservationData.Reserver.Phone = "00011113333";
        reservationData.Reserver.PostCode = "422-8062";
        reservationData.Reserver.Name = "ユーザ";
        reservationData.Reserver.Kana = "ゆーざ";
        reservationData.Reserver.RoomGroupIndex = 0;

        reservationData.MainUser.Address1 = "静岡県";
        reservationData.MainUser.Address2 = "静岡市駿河区稲川";
        reservationData.MainUser.Address3 = "3丁目2番23号";
        reservationData.MainUser.AppDateId = reservationData.CheckInDate;
        reservationData.MainUser.Birthday = 19910101;
        reservationData.MainUser.Gender = "0";
        reservationData.MainUser.Phone = "00022224444";
        reservationData.MainUser.PostCode = "422-8062";
        reservationData.MainUser.Name = "メインユーザ";
        reservationData.MainUser.Kana = "めいんゆーざ";
        reservationData.MainUser.RoomGroupIndex = 0;

        reservationData.Plan.Name = "海に浮かぶ富士を望む露天風呂付";
        reservationData.RoomGroup.Name = "露天風呂付";

        // 予約明細（宿泊ｘ部屋）

        var datas = new List<ReservationPlanRoomGroupAppDateData>();

        //for (int restIndex = 0; restIndex < 2; restIndex++)
        for (var restIndex = 0; restIndex < reservationData.RestNumber; restIndex++)
        {
            //var appDateId = AppDate.GetID(DateTime.Now.AddDays(30).AddDays(restIndex));
            var appDateId = AppDate.GetId(AppDate.GetDateTime(reservationData.CheckInDate).AddDays(restIndex));

            //for (int roomGroupIndex = 0; roomGroupIndex < 3; roomGroupIndex++)
            for (var roomGroupIndex = 0; roomGroupIndex < reservationData.RoomNumber; roomGroupIndex++)
            {
                var data = new ReservationPlanRoomGroupAppDateData
                {
                    AppDateId = appDateId,
                    RoomGroupIndex = roomGroupIndex,
                    ReservationRoomGroupAppDatePersonAgeTypes = new(),
                    ReservationRoomAppDateOptionItems = new()
                };

                // 大人
                data.ReservationRoomGroupAppDatePersonAgeTypes?.Add(
                    BuildReservationRoomGroupAppDatePersonAgeTypeData(
                        restIndex,
                        appDateId,
                        roomGroupIndex,
                        1,
                        "大人",
                        14000,
                        2,
                        1
                    )
                );

                // 子供
                data.ReservationRoomGroupAppDatePersonAgeTypes?.Add(
                    BuildReservationRoomGroupAppDatePersonAgeTypeData(
                        restIndex,
                        appDateId,
                        roomGroupIndex,
                        2,
                        "子供",
                        7000,
                        2,
                        0
                    )
                );

                // 幼児(食事・布団)
                data.ReservationRoomGroupAppDatePersonAgeTypes?.Add(
                    BuildReservationRoomGroupAppDatePersonAgeTypeData(
                        restIndex,
                        appDateId,
                        roomGroupIndex,
                        3,
                        "幼児(食事・布団)",
                        4000,
                        4,
                        2
                    )
                );

                // 幼児(食事)
                data.ReservationRoomGroupAppDatePersonAgeTypes?.Add(
                    BuildReservationRoomGroupAppDatePersonAgeTypeData(
                        restIndex,
                        appDateId,
                        roomGroupIndex,
                        4,
                        "幼児(食事)",
                        3000,
                        4,
                        1
                    )
                );

                // 幼児(布団)
                data.ReservationRoomGroupAppDatePersonAgeTypes?.Add(
                    BuildReservationRoomGroupAppDatePersonAgeTypeData(
                        restIndex,
                        appDateId,
                        roomGroupIndex,
                        5,
                        "幼児(布団)",
                        2000,
                        3,
                        2
                    )
                );

                // 幼児
                data.ReservationRoomGroupAppDatePersonAgeTypes?.Add(
                    BuildReservationRoomGroupAppDatePersonAgeTypeData(
                        restIndex,
                        appDateId,
                        roomGroupIndex,
                        6,
                        "幼児",
                        1000,
                        2,
                        1
                    )
                );

                // オプション
                for (var oi = 0; oi < 5; oi++)
                {
                    var optionId = oi;
                    var number = restIndex + roomGroupIndex + 1;

                    var b = new ReservationOptionItemData
                    {
                        Id = optionId,
                        Name = "オプション" + optionId,
                        Price = 1000 + optionId
                    };

                    var optionItem = new ReservationRoomAppDateOptionItemData
                    {
                        AppDateId = appDateId,
                        Number = number,
                        OptionItem = b,
                        Price = b.Price ?? 0,
                        RoomGroupIndex = roomGroupIndex,
                        TotalPrice = b.Price ?? 0 * number
                    };
                    data.ReservationRoomAppDateOptionItems?.Add(optionItem);
                }

                datas.Add(data);
            }
        }

        reservationData.ReservationPlanRoomGroupAppDates = datas;

        //

        reservation.Facility = facility;
        reservation.ReservationData = reservationData;
        //
        ApplicationName = applicationName;
        Reservation = reservation;
    }

    private ReservationRoomGroupAppDatePersonAgeTypeData BuildReservationRoomGroupAppDatePersonAgeTypeData(
        int restIndex,
        long appDateId,
        int roomGroupIndex,
        int id,
        string name,
        int price,
        int number,
        int maleNumber
    )
    {
        var reservationPersonAgeTypeData = new ReservationPersonAgeTypeData
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
        var spaTax = 150;

        var reservationRoomGroupAppDatePersonAgeTypeData = new ReservationRoomGroupAppDatePersonAgeTypeData
        {
            AppDateId = appDateId,
            RestIndex = restIndex,
            RoomGroupIndex = roomGroupIndex,
            PersonAgeType = reservationPersonAgeTypeData,
            Persons = number,
            MalePersons = maleNumber,
            FemalePersons = femaleNumber,
            NonePeersons = 0,
            Price = price,
            SpaTax = spaTax,
            TotalPrice = price * number,
            TotalSpaTax = spaTax * number
        };

        return reservationRoomGroupAppDatePersonAgeTypeData;
    }
}

public class IO10002TemplateFormat
{
    public string Subject { get; set; } = "*IO10002 ご予約成立";

    public string Body { get; set; } = "*IO10002 本メールは宿泊施設向けの内容です。\n本予約は予約成立されました";

    public string Url { get; set; } = "http://";
}
