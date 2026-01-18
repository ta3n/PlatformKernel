using System.Xml.Serialization;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Liberty.SysException;
using Newtonsoft.Json;
using static Liberty.Reservation.Manager.Distribution.WebAPI.Application.Mappings.BookingMapperProfile;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Responses;

[XmlRoot("response")]
public class GetBookingResponse
{
    [XmlElement("hotel")]
    [JsonProperty("hotel")]
    public List<Hotel> Hotels { get; set; } = [];

    [XmlAttribute("error_code")]
    [JsonProperty("error_code")]
    public string? ErrorCodeString
    {
        get => ErrorCode?.ToString();
        set => ErrorCode = Enum.TryParse(value, out ErrorCode result) ? result : null;
    }

    [XmlIgnore]
    [JsonIgnore]
    public ErrorCode? ErrorCode { get; set; }

    public class Hotel
    {
        /// <summary>施設番号　★必須</summary>
        [XmlAttribute("id")]
        [JsonProperty("id")]
        public string? HotelId { get; set; }

        [XmlElement("booking")]
        [JsonProperty("booking")]
        public List<Booking> Bookings { get; set; } = [];
    }

    public class Booking
    {
        /// <summary>予約番号　★必須</summary>
        [XmlAttribute("id")]
        [JsonProperty("id")]
        public string? BookingId { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public long FacilityId { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public string? FacilityCode { get; set; }

        /// <summary>予約受付日時　★必須(yyyy-MM-dd HH:mm:ss)</summary>
        [XmlElement("booking_time")]
        [JsonProperty("booking_time")]
        public string BookingTime { get; set; } = string.Empty;

        /// <summary>予約受付日時フォーマット</summary>
        public const string BookingTimeFormat = "yyyy-MM-dd HH:mm:ss";

        /// <summary>プラン名　★必須</summary>
        [XmlElement("plan_name")]
        [JsonProperty("plan_name")]
        public string PlanName { get; set; } = string.Empty;

        /// <summary>部屋番号　★必須</summary>
        [XmlElement("room_id")]
        [JsonProperty("room_id")]
        public string? RoomId { get; set; }

        /// <summary>部屋数　★必須</summary>
        [XmlElement("rooms")]
        [JsonProperty("rooms")]
        public int RoomCount { get; set; }

        /// <summary>宿泊日数　★必須</summary>
        [XmlElement("stay_days")]
        [JsonProperty("stay_days")]
        public int StayDaysCount { get; set; }

        /// <summary>チェックイン日　★必須　(yyyy-MM-dd)</summary>
        [XmlElement("arrive_date")]
        [JsonProperty("arrive_date")]
        public string ArriveDate { get; set; } = string.Empty;

        /// <summary>チェックイン時間　★必須　(HH:mm:ss)</summary>
        [XmlElement("arrive_time")]
        [JsonProperty("arrive_time")]
        public string ArriveTime { get; set; } = string.Empty;

        /// <summary>チェックイン時間初期値</summary>
        public const string ArriveTimeDefault = "00:00:00";

        /// <summary>総利用金額　★必須</summary>
        [XmlElement("total_price")]
        [JsonProperty("total_price")]
        public decimal TotalPrice { get; set; }

        [XmlElement("updated_at")]
        [JsonProperty("updated_at")]
        public string UpdatedAt { get; set; } = string.Empty;

        [XmlElement("update_count")]
        [JsonProperty("update_count")]
        public int UpdateCount { get; set; }

        /// <summary>予約状況　★必須</summary>
        [XmlElement("booking_status")]
        [JsonProperty("booking_status")]
        public EnumBookingStatus BookingStatus { get; set; }

        [XmlArray("reservation_statuses")]
        [XmlArrayItem("reservation_status")]
        [JsonProperty("reservation_statuses")]
        public List<BookingState> BookingStates =>
        [
            new()
            {
                Status = BookingStatus switch
                {
                    EnumBookingStatus.Reserve => "Reserve",
                    EnumBookingStatus.Modified => "Modified",
                    EnumBookingStatus.Cancel => "Cancel",
                    _ => ""
                }
            }
        ];

        [XmlElement("booking_cancelled_datetime")]
        [JsonProperty("booking_cancelled_datetime")]
        public string CancelledDateTime { get; set; } = string.Empty;

        [XmlElement("noshow")]
        [JsonProperty("noshow")]
        public bool NoShow { get; set; }

        [XmlElement("noshow_datetime")]
        [JsonProperty("noshow_datetime")]
        public string NoShowDateTime { get; set; } = string.Empty;

        [XmlIgnore]
        [JsonIgnore]
        public BookingData? BookingData { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public List<AddItem> AddItems
        {
            get
            {
                if (BookingData is null)
                {
                    return [];
                }

                var addItems = new List<AddItem>();

                if (BookingData.PlanQuestions is not null)
                {
                    addItems.AddRange(
                        BookingData.PlanQuestions.Select(
                            q => new AddItem
                            {
                                Question = q.Name,
                                Answer = q.AnswerData
                            }
                        )
                    );
                }

                if (BookingData.OptionQuestions is not null)
                {
                    addItems.AddRange(
                        BookingData.OptionQuestions.Select(
                            q => new AddItem
                            {
                                Question = q.Name,
                                Answer = q.AnswerData
                            }
                        )
                    );
                }

                return addItems;
            }
        }

        /// <summary>追加項目回答1</summary>
        private AddItem? _addItem1;

        [XmlElement("add_item1")]
        [JsonProperty("add_item1")]
        public AddItem? AddItem1
        {
            get => _addItem1 ??= AddItems.Count > 0 ? AddItems[0] : null;
            set => _addItem1 = value;
        }

        private AddItem? _addItem2;

        [XmlElement("add_item2")]
        [JsonProperty("add_item2")]
        public AddItem? AddItem2
        {
            get => _addItem2 ??= AddItems.Count > 1 ? AddItems[1] : null;
            set => _addItem2 = value;
        }

        private AddItem? _addItem3;

        [XmlElement("add_item3")]
        [JsonProperty("add_item3")]
        public AddItem? AddItem3
        {
            get => _addItem3 ??= AddItems.Count > 2 ? AddItems[2] : null;
            set => _addItem3 = value;
        }

        private AddItem? _addItem4;

        [XmlElement("add_item4")]
        [JsonProperty("add_item4")]
        public AddItem? AddItem4
        {
            get => _addItem4 ??= AddItems.Count > 3 ? AddItems[3] : null;
            set => _addItem4 = value;
        }

        private AddItem? _addItem5;

        [XmlElement("add_item5")]
        [JsonProperty("add_item5")]
        public AddItem? AddItem5
        {
            get => _addItem5 ??= AddItems.Count > 4 ? AddItems[4] : null;
            set => _addItem5 = value;
        }

        private AddItem? _addItem6;

        [XmlElement("add_item6")]
        [JsonProperty("add_item6")]
        public AddItem? AddItem6
        {
            get => _addItem6 ??= AddItems.Count > 5 ? AddItems[5] : null;
            set => _addItem6 = value;
        }

        private AddItem? _addItem7;

        [XmlElement("add_item7")]
        [JsonProperty("add_item7")]
        public AddItem? AddItem7
        {
            get => _addItem7 ??= AddItems.Count > 6 ? AddItems[6] : null;
            set => _addItem7 = value;
        }

        private AddItem? _addItem8;

        [XmlElement("add_item8")]
        [JsonProperty("add_item8")]
        public AddItem? AddItem8
        {
            get => _addItem8 ??= AddItems.Count > 7 ? AddItems[7] : null;
            set => _addItem8 = value;
        }

        private AddItem? _addItem9;

        [XmlElement("add_item9")]
        [JsonProperty("add_item9")]
        public AddItem? AddItem9
        {
            get => _addItem9 ??= AddItems.Count > 8 ? AddItems[8] : null;
            set => _addItem9 = value;
        }

        private AddItem? _addItem10;

        [XmlElement("add_item10")]
        [JsonProperty("add_item10")]
        public AddItem? AddItem10
        {
            get => _addItem10 ??= AddItems.Count > 9 ? AddItems[9] : null;
            set => _addItem10 = value;
        }

        private AddItem? _addItem11;

        [XmlElement("add_item11")]
        [JsonProperty("add_item11")]
        public AddItem? AddItem11
        {
            get => _addItem11 ??= AddItems.Count > 10 ? AddItems[10] : null;
            set => _addItem11 = value;
        }

        private AddItem? _addItem12;

        [XmlElement("add_item12")]
        [JsonProperty("add_item12")]
        public AddItem? AddItem12
        {
            get => _addItem12 ??= AddItems.Count > 11 ? AddItems[11] : null;
            set => _addItem12 = value;
        }

        private AddItem? _addItem13;

        [XmlElement("add_item13")]
        [JsonProperty("add_item13")]
        public AddItem? AddItem13
        {
            get => _addItem13 ??= AddItems.Count > 12 ? AddItems[12] : null;
            set => _addItem13 = value;
        }

        private AddItem? _addItem14;

        [XmlElement("add_item14")]
        [JsonProperty("add_item14")]
        public AddItem? AddItem14
        {
            get => _addItem14 ??= AddItems.Count > 13 ? AddItems[13] : null;
            set => _addItem14 = value;
        }

        private AddItem? _addItem15;

        [XmlElement("add_item15")]
        [JsonProperty("add_item15")]
        public AddItem? AddItem15
        {
            get => _addItem15 ??= AddItems.Count > 14 ? AddItems[14] : null;
            set => _addItem15 = value;
        }

        private AddItem? _addItem16;

        [XmlElement("add_item16")]
        [JsonProperty("add_item16")]
        public AddItem? AddItem16
        {
            get => _addItem16 ??= AddItems.Count > 15 ? AddItems[15] : null;
            set => _addItem16 = value;
        }

        private AddItem? _addItem17;

        [XmlElement("add_item17")]
        [JsonProperty("add_item17")]
        public AddItem? AddItem17
        {
            get => _addItem17 ??= AddItems.Count > 16 ? AddItems[16] : null;
            set => _addItem17 = value;
        }

        private AddItem? _addItem18;

        [XmlElement("add_item18")]
        [JsonProperty("add_item18")]
        public AddItem? AddItem18
        {
            get => _addItem18 ??= AddItems.Count > 17 ? AddItems[17] : null;
            set => _addItem18 = value;
        }

        private AddItem? _addItem19;

        [XmlElement("add_item19")]
        [JsonProperty("add_item19")]
        public AddItem? AddItem19
        {
            get => _addItem19 ??= AddItems.Count > 18 ? AddItems[18] : null;
            set => _addItem19 = value;
        }

        private AddItem? _addItem20;

        [XmlElement("add_item20")]
        [JsonProperty("add_item20")]
        public AddItem? AddItem20
        {
            get => _addItem20 ??= AddItems.Count > 19 ? AddItems[19] : null;
            set => _addItem20 = value;
        }

        private AddItem? _addItem21;

        [XmlElement("add_item21")]
        [JsonProperty("add_item21")]
        public AddItem? AddItem21
        {
            get => _addItem21 ??= AddItems.Count > 20 ? AddItems[20] : null;
            set => _addItem21 = value;
        }

        private AddItem? _addItem22;

        [XmlElement("add_item22")]
        [JsonProperty("add_item22")]
        public AddItem? AddItem22
        {
            get => _addItem22 ??= AddItems.Count > 21 ? AddItems[21] : null;
            set => _addItem22 = value;
        }

        private AddItem? _addItem23;

        [XmlElement("add_item23")]
        [JsonProperty("add_item23")]
        public AddItem? AddItem23
        {
            get => _addItem23 ??= AddItems.Count > 22 ? AddItems[22] : null;
            set => _addItem23 = value;
        }

        private AddItem? _addItem24;

        [XmlElement("add_item24")]
        [JsonProperty("add_item24")]
        public AddItem? AddItem24
        {
            get => _addItem24 ??= AddItems.Count > 23 ? AddItems[23] : null;
            set => _addItem24 = value;
        }

        private AddItem? _addItem25;

        [XmlElement("add_item25")]
        [JsonProperty("add_item25")]
        public AddItem? AddItem25
        {
            get => _addItem25 ??= AddItems.Count > 24 ? AddItems[24] : null;
            set => _addItem25 = value;
        }

        private AddItem? _addItem26;

        [XmlElement("add_item26")]
        [JsonProperty("add_item26")]
        public AddItem? AddItem26
        {
            get => _addItem26 ??= AddItems.Count > 25 ? AddItems[25] : null;
            set => _addItem26 = value;
        }

        private AddItem? _addItem27;

        [XmlElement("add_item27")]
        [JsonProperty("add_item27")]
        public AddItem? AddItem27
        {
            get => _addItem27 ??= AddItems.Count > 26 ? AddItems[26] : null;
            set => _addItem27 = value;
        }

        private AddItem? _addItem28;

        [XmlElement("add_item28")]
        [JsonProperty("add_item28")]
        public AddItem? AddItem28
        {
            get => _addItem28 ??= AddItems.Count > 27 ? AddItems[27] : null;
            set => _addItem28 = value;
        }

        private AddItem? _addItem29;

        [XmlElement("add_item29")]
        [JsonProperty("add_item29")]
        public AddItem? AddItem29
        {
            get => _addItem29 ??= AddItems.Count > 28 ? AddItems[28] : null;
            set => _addItem29 = value;
        }

        private AddItem? _addItem30;

        [XmlElement("add_item30")]
        [JsonProperty("add_item30")]
        public AddItem? AddItem30
        {
            get => _addItem30 ??= AddItems.Count > 29 ? AddItems[29] : null;
            set => _addItem30 = value;
        }

        private AddItem? _addItem31;

        [XmlElement("add_item31")]
        [JsonProperty("add_item31")]
        public AddItem? AddItem31
        {
            get => _addItem31 ??= AddItems.Count > 30 ? AddItems[30] : null;
            set => _addItem31 = value;
        }

        private AddItem? _addItem32;

        [XmlElement("add_item32")]
        [JsonProperty("add_item32")]
        public AddItem? AddItem32
        {
            get => _addItem32 ??= AddItems.Count > 31 ? AddItems[31] : null;
            set => _addItem32 = value;
        }

        private AddItem? _addItem33;

        [XmlElement("add_item33")]
        [JsonProperty("add_item33")]
        public AddItem? AddItem33
        {
            get => _addItem33 ??= AddItems.Count > 32 ? AddItems[32] : null;
            set => _addItem33 = value;
        }

        private AddItem? _addItem34;

        [XmlElement("add_item34")]
        [JsonProperty("add_item34")]
        public AddItem? AddItem34
        {
            get => _addItem34 ??= AddItems.Count > 33 ? AddItems[33] : null;
            set => _addItem34 = value;
        }

        private AddItem? _addItem35;

        [XmlElement("add_item35")]
        [JsonProperty("add_item35")]
        public AddItem? AddItem35
        {
            get => _addItem35 ??= AddItems.Count > 34 ? AddItems[34] : null;
            set => _addItem35 = value;
        }

        private AddItem? _addItem36;

        [XmlElement("add_item36")]
        [JsonProperty("add_item36")]
        public AddItem? AddItem36
        {
            get => _addItem36 ??= AddItems.Count > 35 ? AddItems[35] : null;
            set => _addItem36 = value;
        }

        private AddItem? _addItem37;

        [XmlElement("add_item37")]
        [JsonProperty("add_item37")]
        public AddItem? AddItem37
        {
            get => _addItem37 ??= AddItems.Count > 36 ? AddItems[36] : null;
            set => _addItem37 = value;
        }

        private AddItem? _addItem38;

        [XmlElement("add_item38")]
        [JsonProperty("add_item38")]
        public AddItem? AddItem38
        {
            get => _addItem38 ??= AddItems.Count > 37 ? AddItems[37] : null;
            set => _addItem38 = value;
        }

        private AddItem? _addItem39;

        [XmlElement("add_item39")]
        [JsonProperty("add_item39")]
        public AddItem? AddItem39
        {
            get => _addItem39 ??= AddItems.Count > 38 ? AddItems[38] : null;
            set => _addItem39 = value;
        }

        private AddItem? _addItem40;

        [XmlElement("add_item40")]
        [JsonProperty("add_item40")]
        public AddItem? AddItem40
        {
            get => _addItem40 ??= AddItems.Count > 39 ? AddItems[39] : null;
            set => _addItem40 = value;
        }

        private AddItem? _addItem41;

        [XmlElement("add_item41")]
        [JsonProperty("add_item41")]
        public AddItem? AddItem41
        {
            get => _addItem41 ??= AddItems.Count > 40 ? AddItems[40] : null;
            set => _addItem41 = value;
        }

        private AddItem? _addItem42;

        [XmlElement("add_item42")]
        [JsonProperty("add_item42")]
        public AddItem? AddItem42
        {
            get => _addItem42 ??= AddItems.Count > 41 ? AddItems[41] : null;
            set => _addItem42 = value;
        }

        private AddItem? _addItem43;

        [XmlElement("add_item43")]
        [JsonProperty("add_item43")]
        public AddItem? AddItem43
        {
            get => _addItem43 ??= AddItems.Count > 42 ? AddItems[42] : null;
            set => _addItem43 = value;
        }

        private AddItem? _addItem44;

        [XmlElement("add_item44")]
        [JsonProperty("add_item44")]
        public AddItem? AddItem44
        {
            get => _addItem44 ??= AddItems.Count > 43 ? AddItems[43] : null;
            set => _addItem44 = value;
        }

        private AddItem? _addItem45;

        [XmlElement("add_item45")]
        [JsonProperty("add_item45")]
        public AddItem? AddItem45
        {
            get => _addItem45 ??= AddItems.Count > 44 ? AddItems[44] : null;
            set => _addItem45 = value;
        }

        private AddItem? _addItem46;

        [XmlElement("add_item46")]
        [JsonProperty("add_item46")]
        public AddItem? AddItem46
        {
            get => _addItem46 ??= AddItems.Count > 45 ? AddItems[45] : null;
            set => _addItem46 = value;
        }

        private AddItem? _addItem47;

        [XmlElement("add_item47")]
        [JsonProperty("add_item47")]
        public AddItem? AddItem47
        {
            get => _addItem47 ??= AddItems.Count > 46 ? AddItems[46] : null;
            set => _addItem47 = value;
        }

        private AddItem? _addItem48;

        [XmlElement("add_item48")]
        [JsonProperty("add_item48")]
        public AddItem? AddItem48
        {
            get => _addItem48 ??= AddItems.Count > 47 ? AddItems[47] : null;
            set => _addItem48 = value;
        }

        private AddItem? _addItem49;

        [XmlElement("add_item49")]
        [JsonProperty("add_item49")]
        public AddItem? AddItem49
        {
            get => _addItem49 ??= AddItems.Count > 48 ? AddItems[48] : null;
            set => _addItem49 = value;
        }

        private AddItem? _addItem50;

        [XmlElement("add_item50")]
        [JsonProperty("add_item50")]
        public AddItem? AddItem50
        {
            get => _addItem50 ??= AddItems.Count > 49 ? AddItems[49] : null;
            set => _addItem50 = value;
        }

        /// <summary>注意事項</summary>
        [XmlElement("attention")]
        [JsonProperty("attention")]
        public string Attention { get; set; } = string.Empty;

        [XmlElement("rate_type")]
        [JsonProperty("rate_type")]
        public string PlanType { get; set; } = string.Empty;

        /// <summary>予約者氏名　★必須</summary>
        [XmlElement("guest_name")]
        [JsonProperty("guest_name")]
        public string GuestName { get; set; } = string.Empty;

        /// <summary>予約者フリガナ　★必須</summary>
        [XmlElement("furigana")]
        [JsonProperty("furigana")]
        public string Furigana { get; set; } = string.Empty;

        /// <summary>予約者郵便番号　★必須　(000-0000)</summary>
        [XmlElement("postcode")]
        [JsonProperty("postcode")]
        public string PostCode { get; set; } = string.Empty;

        /// <summary>予約者住所1　★必須</summary>
        [XmlElement("address1")]
        [JsonProperty("address1")]
        public string Address1 { get; set; } = string.Empty;

        /// <summary>予約者住所2　★必須</summary>
        [XmlElement("address2")]
        [JsonProperty("address2")]
        public string Address2 { get; set; } = string.Empty;

        /// <summary>予約者メールアドレス</summary>
        [XmlElement("mail")]
        [JsonProperty("mail")]
        public string Mail { get; set; } = string.Empty;

        /// <summary>予約者電話番号</summary>
        [XmlElement("tel")]
        [JsonProperty("tel")]
        public string Tel { get; set; } = string.Empty;

        /// <summary>予約者携帯電話番号</summary>
        [XmlElement("mobile_tel")]
        [JsonProperty("mobile_tel")]
        public string? MobileTel { get; set; }

        /// <summary>予約者性別　★必須</summary>
        [XmlElement("sex")]
        [JsonProperty("sex")]
        public EnumSex Sex { get; set; }

        /// <summary>予約者年代層　★必須</summary>
        [XmlElement("age")]
        [JsonProperty("age")]
        public EnumAge Age { get; set; }

        /// <summary>滞在者氏名</summary>
        [XmlElement("stay_name")]
        [JsonProperty("stay_name")]
        public string StayName { get; set; } = string.Empty;

        /// <summary>滞在者フリガナ</summary>
        [XmlElement("stay_furigana")]
        [JsonProperty("stay_furigana")]
        public string StayFurigana { get; set; } = string.Empty;

        /// <summary>滞在者郵便番号　(000-0000)</summary>
        [XmlElement("stay_postcode")]
        [JsonProperty("stay_postcode")]
        public string StayPostCode { get; set; } = string.Empty;

        [XmlElement("payment_type")]
        [JsonProperty("payment_type")]
        public string PaymentType { get; set; } = string.Empty;

        /// <summary>滞在者住所1</summary>
        [XmlElement("stay_address1")]
        [JsonProperty("stay_address1")]
        public string StayAddress1 { get; set; } = string.Empty;

        /// <summary>滞在者住所2</summary>
        [XmlElement("stay_address2")]
        [JsonProperty("stay_address2")]
        public string StayAddress2 { get; set; } = string.Empty;

        /// <summary>滞在者電話番号</summary>
        [XmlElement("stay_tel")]
        [JsonProperty("stay_tel")]
        public string StayTel { get; set; } = string.Empty;

        /// <summary>滞在者携帯電話番号</summary>
        [XmlElement("stay_mobile_tel")]
        [JsonProperty("stay_mobile_tel")]
        public string StayMobileTel { get; set; } = string.Empty;

        /// <summary>滞在者性別</summary>
        [XmlElement("stay_sex")]
        [JsonProperty("stay_sex")]
        public EnumSex StaySex { get; set; }

        /// <summary>滞在者年代層</summary>
        [XmlElement("stay_age")]
        [JsonProperty("stay_age")]
        public EnumAge StayAge { get; set; }

        [XmlElement("date")]
        [JsonProperty("date")]
        public List<UnitPriceAndPeopleCount> UnitPriceAndPeopleCounts { get; set; } = [];
    }

    public class AddItem
    {
        /// <summary>追加項目　質問</summary>
        [XmlAttribute("item")]
        [JsonProperty("item")]
        public string? Question { get; set; } = string.Empty;

        /// <summary>追加項目　回答</summary>
        [XmlText]
        [JsonProperty("answer")]
        public string? Answer { get; set; } = string.Empty;
    }

    public class RoomItem
    {
        [XmlAttribute("rate")]
        [JsonProperty("rate")]
        public decimal Rate { get; set; }

        [XmlText]
        [JsonProperty("count")]
        public long Count { get; set; }
    }

    public class UnitPriceAndPeopleCount
    {
        [XmlIgnore]
        [JsonIgnore]
        public long? BookingDateId { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        private List<RoomItem>? _rooms;

        [XmlIgnore]
        [JsonIgnore]
        public string? PlanType { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        private NameAndRate? _bathtaxAdult;

        [XmlIgnore]
        [JsonIgnore]
        private NameAndRate? _bathtaxChild;

        [XmlIgnore]
        [JsonIgnore]
        private NameAndRate? _bathtaxTeen;

        [XmlIgnore]
        [JsonIgnore]
        public List<AppDatePersonAgeType> PersonAgeTypes { get; set; } = [];

        [XmlAttribute("use_spa_tax")]
        [JsonProperty("use_spa_tax")]
        public bool UseSpaTax { get; set; }

        private string? _targetDate;

        [XmlAttribute("value")]
        [JsonProperty("value")]
        public string TargetDate
        {
            get => _targetDate ??= BookingDateId.HasValue
                ? $"{AppDate.GetDateTime(BookingDateId.Value):yyyy-MM-dd}"
                : string.Empty;

            set => _targetDate = value;
        }

        private List<NameAndRate>? _men;

        [XmlElement("man")]
        [JsonProperty("man")]
        public List<NameAndRate> Men
        {
            get
            {
                if (_men != null)
                {
                    return _men;
                }

                _men =
                [
                    .. PersonAgeTypes
                        .Where(x => x.PersonAgeGroup == PersonAgeGroups.Adult)
                        .GroupBy(x => x.UnitPrice)
                        .Select(
                            g => new NameAndRate
                            {
                                Rate = PlanType == RateTypeLabels.PerPerson ? g.Key : 0,
                                Count = g.Sum(x => x.MaleNumber + x.GenderNoneNumber)
                            }
                        )
                ];

                return _men;
            }
            set => _men = value;
        }

        private List<NameAndRate>? _women;

        [XmlElement("woman")]
        [JsonProperty("woman")]
        public List<NameAndRate> Women
        {
            get
            {
                if (_women != null)
                {
                    return _women;
                }

                _women =
                [
                    .. PersonAgeTypes
                        .Where(x => x.PersonAgeGroup == PersonAgeGroups.Adult)
                        .GroupBy(x => x.UnitPrice)
                        .Select(
                            g => new NameAndRate
                            {
                                Rate = PlanType == RateTypeLabels.PerPerson ? g.Key : 0,
                                Count = g.Sum(x => x.FemaleNumber)
                            }
                        )
                ];

                return _women;
            }
            set => _women = value;
        }

        private List<NameAndRate>? _teens;

        [XmlElement("teen")]
        [JsonProperty("teen")]
        public List<NameAndRate> Teens
        {
            get
            {
                if (_teens != null)
                {
                    return _teens;
                }

                _teens =
                [
                    .. PersonAgeTypes
                        .Where(x => x.PersonAgeGroup == PersonAgeGroups.Teen)
                        .GroupBy(x => x.UnitPrice)
                        .Select(
                            g => new NameAndRate
                            {
                                Rate = g.Key,
                                Count = g.Sum(x => x.Number)
                            }
                        )
                ];

                return _teens;
            }
            set => _teens = value;
        }

        private List<NameAndRate>? _teenBs;

        [XmlElement("teen_b")]
        [JsonProperty("teen_b")]
        public List<NameAndRate> TeenBs
        {
            get
            {
                if (_teenBs != null)
                {
                    return _teenBs;
                }

                _teenBs =
                [
                    .. PersonAgeTypes
                        .Where(x => x.PersonAgeGroup == PersonAgeGroups.TeenB)
                        .GroupBy(x => x.UnitPrice)
                        .Select(
                            g => new NameAndRate
                            {
                                Rate = g.Key,
                                Count = g.Sum(x => x.Number)
                            }
                        )
                ];

                return _teenBs;
            }
            set => _teenBs = value;
        }

        private List<NameAndRate>? _childFoodBeds;

        [XmlElement("child_foodbed")]
        [JsonProperty("child_foodbed")]
        public List<NameAndRate> ChildFoodBeds
        {
            get
            {
                if (_childFoodBeds != null)
                {
                    return _childFoodBeds;
                }

                _childFoodBeds =
                [
                    .. PersonAgeTypes
                        .Where(x => x.PersonAgeGroup == PersonAgeGroups.Child)
                        .Where(x => x.FoodBed == (FoodBeds.Food | FoodBeds.Bed))
                        .GroupBy(x => x.UnitPrice)
                        .Select(
                            g => new NameAndRate
                            {
                                Rate = g.Key,
                                Count = g.Sum(x => x.Number)
                            }
                        )
                ];

                return _childFoodBeds;
            }
            set => _childFoodBeds = value;
        }

        private List<NameAndRate>? _childFoods;

        [XmlElement("child_food")]
        [JsonProperty("child_food")]
        public List<NameAndRate> ChildFoods
        {
            get
            {
                if (_childFoods != null)
                {
                    return _childFoods;
                }

                _childFoods =
                [
                    .. PersonAgeTypes
                        .Where(x => x.PersonAgeGroup == PersonAgeGroups.Child)
                        .Where(x => x.FoodBed == FoodBeds.Food)
                        .GroupBy(x => x.UnitPrice)
                        .Select(
                            g => new NameAndRate
                            {
                                Rate = g.Key,
                                Count = g.Sum(x => x.Number)
                            }
                        )
                ];

                return _childFoods;
            }
            set => _childFoods = value;
        }

        private List<NameAndRate>? _childBeds;

        [XmlElement("child_bed")]
        [JsonProperty("child_bed")]
        public List<NameAndRate> ChildBeds
        {
            get
            {
                if (_childBeds != null)
                {
                    return _childBeds;
                }

                _childBeds =
                [
                    .. PersonAgeTypes
                        .Where(x => x.PersonAgeGroup == PersonAgeGroups.Child)
                        .Where(x => x.FoodBed == FoodBeds.Bed)
                        .GroupBy(x => x.UnitPrice)
                        .Select(
                            g => new NameAndRate
                            {
                                Rate = g.Key,
                                Count = g.Sum(x => x.Number)
                            }
                        )
                ];

                return _childBeds;
            }
            set => _childBeds = value;
        }

        private List<NameAndRate>? _childNeithers;

        [XmlElement("child_neither")]
        [JsonProperty("child_neither")]
        public List<NameAndRate> ChildNeithers
        {
            get
            {
                if (_childNeithers != null)
                {
                    return _childNeithers;
                }

                _childNeithers =
                [
                    .. PersonAgeTypes
                        .Where(x => x.PersonAgeGroup == PersonAgeGroups.Child)
                        .Where(x => x.FoodBed == FoodBeds.None)
                        .GroupBy(x => x.UnitPrice)
                        .Select(
                            g => new NameAndRate
                            {
                                Rate = g.Key,
                                Count = g.Sum(x => x.Number)
                            }
                        )
                ];

                return _childNeithers;
            }
            set => _childNeithers = value;
        }

        private List<NameAndRate>? _babyFoodBeds;

        [XmlElement("baby_foodbed")]
        [JsonProperty("baby_foodbed")]
        public List<NameAndRate> BabyFoodBeds
        {
            get
            {
                if (_babyFoodBeds != null)
                {
                    return _babyFoodBeds;
                }

                _babyFoodBeds =
                [
                    .. PersonAgeTypes
                        .Where(x => x.PersonAgeGroup == PersonAgeGroups.Baby)
                        .Where(x => x.FoodBed == (FoodBeds.Food | FoodBeds.Bed))
                        .GroupBy(x => x.UnitPrice)
                        .Select(
                            g => new NameAndRate
                            {
                                Rate = g.Key,
                                Count = g.Sum(x => x.Number)
                            }
                        )
                ];

                return _babyFoodBeds;
            }
            set => _babyFoodBeds = value;
        }

        private List<NameAndRate>? _babyFoods;

        [XmlElement("baby_food")]
        [JsonProperty("baby_food")]
        public List<NameAndRate> BabyFoods
        {
            get
            {
                if (_babyFoods != null)
                {
                    return _babyFoods;
                }

                _babyFoods =
                [
                    .. PersonAgeTypes
                        .Where(x => x.PersonAgeGroup == PersonAgeGroups.Baby)
                        .Where(x => x.FoodBed == FoodBeds.Food)
                        .GroupBy(x => x.UnitPrice)
                        .Select(
                            g => new NameAndRate
                            {
                                Rate = g.Key,
                                Count = g.Sum(x => x.Number)
                            }
                        )
                ];

                return _babyFoods;
            }
            set => _babyFoods = value;
        }

        private List<NameAndRate>? _babyBeds;

        [XmlElement("baby_bed")]
        [JsonProperty("baby_bed")]
        public List<NameAndRate> BabyBeds
        {
            get
            {
                if (_babyBeds != null)
                {
                    return _babyBeds;
                }

                _babyBeds =
                [
                    .. PersonAgeTypes
                        .Where(x => x.PersonAgeGroup == PersonAgeGroups.Baby)
                        .Where(x => x.FoodBed == FoodBeds.Bed)
                        .GroupBy(x => x.UnitPrice)
                        .Select(
                            g => new NameAndRate
                            {
                                Rate = g.Key,
                                Count = g.Sum(x => x.Number)
                            }
                        )
                ];

                return _babyBeds;
            }
            set => _babyBeds = value;
        }

        private List<NameAndRate>? _babyNeithers;

        [XmlElement("baby_neither")]
        [JsonProperty("baby_neither")]
        public List<NameAndRate> BabyNeithers
        {
            get
            {
                if (_babyNeithers != null)
                {
                    return _babyNeithers;
                }

                _babyNeithers =
                [
                    .. PersonAgeTypes
                        .Where(x => x.PersonAgeGroup == PersonAgeGroups.Baby)
                        .Where(x => x.FoodBed == FoodBeds.None)
                        .GroupBy(x => x.UnitPrice)
                        .Select(
                            g => new NameAndRate
                            {
                                Rate = g.Key,
                                Count = g.Sum(x => x.Number)
                            }
                        )
                ];

                return _babyNeithers;
            }
            set => _babyNeithers = value;
        }

        [XmlElement("bathtax_adult")]
        [JsonProperty("bathtax_adult")]
        public NameAndRate? BathtaxAdult
        {
            get
            {
                if (_bathtaxAdult != null)
                {
                    return _bathtaxAdult;
                }

                var adults = PersonAgeTypes.Where(x => x.PersonAgeGroup == PersonAgeGroups.Adult).ToList();

                _bathtaxAdult = new NameAndRate
                {
                    Count = adults.Sum(x => x.Number),
                    Rate = adults.Select(x => x.SpaTax).FirstOrDefault()
                };

                return _bathtaxAdult;
            }
            set => _bathtaxAdult = value;
        }

        [XmlElement("bathtax_child")]
        [JsonProperty("bathtax_child")]
        public NameAndRate? BathtaxChild
        {
            get
            {
                if (_bathtaxChild != null)
                {
                    return _bathtaxChild;
                }

                var children = PersonAgeTypes.Where(x => x.PersonAgeGroup == PersonAgeGroups.Child).ToList();

                _bathtaxChild = new NameAndRate
                {
                    Count = children.Sum(x => x.Number),
                    Rate = children.Select(x => x.SpaTax).FirstOrDefault()
                };

                return _bathtaxChild;
            }
            set => _bathtaxChild = value;
        }

        [XmlElement("bathtax_teen")]
        [JsonProperty("bathtax_teen")]
        public NameAndRate? BathtaxTeen
        {
            get
            {
                if (_bathtaxTeen != null)
                {
                    return _bathtaxTeen;
                }

                var teens = PersonAgeTypes
                    .Where(x => x.PersonAgeGroup is PersonAgeGroups.Teen or PersonAgeGroups.TeenB)
                    .ToList();

                _bathtaxTeen = new NameAndRate
                {
                    Count = teens.Sum(x => x.Number),
                    Rate = teens.Select(x => x.SpaTax).FirstOrDefault()
                };

                return _bathtaxTeen;
            }
            set => _bathtaxTeen = value;
        }

        /// <summary>日帰り入湯税　★必須</summary>
        [XmlElement("bathtax_date")]
        [JsonProperty("bathtax_date")]
        public NameAndRate? BathtaxDate { get; set; }

        [XmlElement("add_menu")]
        [JsonProperty("add_menu")]
        public List<AddMenu> AddMenus { get; set; } = [];

        [XmlIgnore]
        [JsonIgnore]
        public BookingData? BookingData { get; set; }

        [XmlElement("room")]
        [JsonProperty("room")]
        public List<RoomItem> Rooms
        {
            get
            {
                _rooms ??= GetRoomsGroupedByPrice();
                return _rooms;
            }
            set => _rooms = value;
        }

        private List<RoomItem> GetRoomsGroupedByPrice()
        {
            if (
                BookingDateId is null || BookingData?.AppDates is null || PlanType is RateTypeLabels.PerPerson
            )
            {
                return [];
            }

            var rooms = BookingData.AppDates
                .Find(ad => ad.AppDateId == BookingDateId)
                ?.Rooms;

            return rooms?.Select(
                        r =>
                            new RoomItem
                            {
                                Rate = r.RoomPrice,
                                Count = 1
                            }
                    )
                    .ToList()
                ?? [];
        }

        // Use ShouldSerializeProperty to control serialization of the property.
        // Detail: https://learn.microsoft.com/en-us/dotnet/desktop/winforms/controls-design/how-to-designer-properties-shouldserialize-reset

        /// Determines whether the BathtaxAdult property should be serialized.
        /// <returns>
        /// True if the property should be serialized; otherwise, false. The property is serialized
        /// if UseSpaTax is true and BathtaxAdult has a valid count greater than 0 and a rate greater than 0.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "CodeQuality",
            "S1144:Unused private methods should be removed",
            Justification = "Used implicitly by serialization via ShouldSerialize pattern"
        )]
        public bool ShouldSerializeBathtaxAdult()
        {
            return UseSpaTax;
        }

        /// Determines whether the BathtaxChild property should be serialized.
        /// <returns>
        /// True if spa tax is enabled and the BathtaxChild property has a count greater than zero and a rate greater than zero; otherwise, false.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "CodeQuality",
            "S1144:Unused private methods should be removed",
            Justification = "Used implicitly by serialization via ShouldSerialize pattern"
        )]
        public bool ShouldSerializeBathtaxChild()
        {
            return UseSpaTax;
        }

        /// Determines whether the BathtaxTeen property should be serialized into the output.
        /// This method checks if the UseSpaTax property is set to true and the BathtaxTeen property
        /// is not null or empty, and if the associated rate and count are greater than zero.
        /// <returns>
        /// True if UseSpaTax is true and BathtaxTeen is not null with a positive rate and count;
        /// otherwise, false.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "CodeQuality",
            "S1144:Unused private methods should be removed",
            Justification = "Used implicitly by serialization via ShouldSerialize pattern"
        )]
        public bool ShouldSerializeBathtaxTeen()
        {
            return UseSpaTax;
        }

        /// Determines whether the BathtaxDate property should be serialized in the response.
        /// <returns>
        /// True if the BathtaxDate property should be serialized, false otherwise.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "CodeQuality",
            "S1144:Unused private methods should be removed",
            Justification = "Used implicitly by serialization via ShouldSerialize pattern"
        )]
        public bool ShouldSerializeBathtaxDate()
        {
            return UseSpaTax;
        }
    }

    public record AppDatePersonAgeType(
        int RestIndex,
        int RoomGroupIndex,
        decimal UnitPrice,
        decimal SpaTax,
        int MaleNumber,
        int FemaleNumber,
        int GenderNoneNumber,
        int Number,
        PersonAgeGroups PersonAgeGroup,
        FoodBeds FoodBed
    );

    public class AddMenu
    {
        /// <summary>オプション名</summary>
        [XmlAttribute("name")]
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>オプション区分ID</summary>
        [XmlAttribute("type")]
        [JsonProperty("type")]
        public long Type { get; set; }

        /// <summary>単価</summary>
        [XmlAttribute("rate")]
        [JsonProperty("rate")]
        public decimal Rate { get; set; }

        /// <summary>人数</summary>
        [XmlText]
        [JsonProperty("count")]
        public long Count { get; set; }
    }

    public class NameAndRate
    {
        /// <summary>単価</summary>
        [XmlAttribute("rate")]
        [JsonProperty("rate")]
        public decimal Rate { get; set; }

        /// <summary>人数</summary>
        [XmlText]
        [JsonProperty("count")]
        public long Count { get; set; }
    }

    public class BookingState
    {
        [XmlText]
        [JsonProperty("reservation_status")]
        public string Status { get; set; } = string.Empty;
    }

    public enum EnumBookingStatus
    {
        [XmlEnum("新規予約")]
        [JsonProperty("新規予約")]
        Reserve,

        [XmlEnum("cancel")]
        [JsonProperty("cancel")]
        Cancel,

        [XmlEnum("予約変更")]
        [JsonProperty("予約変更")]
        Modified
    }

    public enum EnumCloseType
    {
        /// <summary>販売</summary>
        [XmlEnum("0")]
        [JsonProperty("0")]
        Sale,

        /// <summary>売止</summary>
        [XmlEnum("1")]
        [JsonProperty("1")]
        Stop
    }

    public enum EnumAge
    {
        /// <summary>未選択</summary>
        [XmlEnum("未選択")]
        [JsonProperty("未選択")]
        Unselected,

        /// <summary>10代</summary>
        [XmlEnum("10代")]
        [JsonProperty("10代")]
        Age10,

        /// <summary>20代</summary>
        [XmlEnum("20代")]
        [JsonProperty("20代")]
        Age20,

        /// <summary>30代</summary>
        [XmlEnum("30代")]
        [JsonProperty("30代")]
        Age30,

        /// <summary>40代</summary>
        [XmlEnum("40代")]
        [JsonProperty("40代")]
        Age40,

        /// <summary>50代</summary>
        [XmlEnum("50代")]
        [JsonProperty("50代")]
        Age50,

        /// <summary>60代</summary>
        [XmlEnum("60代")]
        [JsonProperty("60代")]
        Age60,

        /// <summary>70代</summary>
        [XmlEnum("70代")]
        [JsonProperty("70代")]
        Age70,

        /// <summary>80代</summary>
        [XmlEnum("80代")]
        [JsonProperty("80代")]
        Age80
    }

    public enum EnumSex
    {
        [XmlEnum("2")]
        [JsonProperty("2")]
        Unselected,

        /// <summary>男性</summary>
        [XmlEnum("0")]
        [JsonProperty("0")]
        Male,

        /// <summary>女性</summary>
        [XmlEnum("1")]
        [JsonProperty("1")]
        Female
    }
}
