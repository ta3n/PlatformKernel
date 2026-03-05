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
        public List<AddItem> AddItems { get; set; } = [];

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
        public string Question { get; set; } = string.Empty;

        /// <summary>追加項目　回答</summary>
        [XmlText]
        [JsonProperty("answer")]
        public string Answer { get; set; } = string.Empty;
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
        private NameAndRate? _man;

        [XmlIgnore]
        [JsonIgnore]
        private NameAndRate? _woman;

        [XmlIgnore]
        [JsonIgnore]
        private NameAndRate? _teen;

        [XmlIgnore]
        [JsonIgnore]
        private NameAndRate? _teenB;

        [XmlIgnore]
        [JsonIgnore]
        private NameAndRate? _childFoodBed;

        [XmlIgnore]
        [JsonIgnore]
        private NameAndRate? _childFood;

        [XmlIgnore]
        [JsonIgnore]
        private NameAndRate? _childBed;

        [XmlIgnore]
        [JsonIgnore]
        private NameAndRate? _childNeither;

        [XmlIgnore]
        [JsonIgnore]
        private NameAndRate? _babyFoodBed;

        [XmlIgnore]
        [JsonIgnore]
        private NameAndRate? _babyFood;

        [XmlIgnore]
        [JsonIgnore]
        private NameAndRate? _babyBed;

        [XmlIgnore]
        [JsonIgnore]
        private NameAndRate? _babyNeither;

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

        [XmlElement("man")]
        [JsonProperty("man")]
        public NameAndRate? Man
        {
            get
            {
                _man ??= new NameAndRate
                {
                    Count = PersonAgeTypes
                        .Where(x => x.PersonAgeGroup == PersonAgeGroups.Adult)
                        .Select(x => x.MaleNumber + x.GenderNoneNumber)
                        .Sum(),
                    Rate = PlanType == RateTypeLabels.PerPerson
                        ? PersonAgeTypes
                            .Where(x => x.PersonAgeGroup == PersonAgeGroups.Adult)
                            .Where(x => x.MaleNumber + x.GenderNoneNumber > 0)
                            .Select(x => x.UnitPrice)
                            .FirstOrDefault()
                        : 0
                };
                return _man;
            }
            set => _man = value;
        }

        [XmlElement("woman")]
        [JsonProperty("woman")]
        public NameAndRate? Woman
        {
            get
            {
                _woman ??= new NameAndRate
                {
                    Count = PersonAgeTypes
                        .Where(x => x.PersonAgeGroup == PersonAgeGroups.Adult)
                        .Select(x => x.FemaleNumber)
                        .Sum(),
                    Rate = PlanType == RateTypeLabels.PerPerson
                        ? PersonAgeTypes
                            .Where(x => x.PersonAgeGroup == PersonAgeGroups.Adult)
                            .Where(x => x.FemaleNumber > 0)
                            .Select(x => x.UnitPrice)
                            .FirstOrDefault()
                        : 0
                };
                return _woman;
            }
            set => _woman = value;
        }

        /// <summary>小学生高学年　★必須</summary>
        [XmlElement("teen")]
        [JsonProperty("teen")]
        public NameAndRate? Teen
        {
            get
            {
                _teen ??= new NameAndRate
                {
                    Count = PersonAgeTypes.Where(x => x.PersonAgeGroup == PersonAgeGroups.Teen)
                        .Select(x => x.Number)
                        .Sum(),
                    Rate = PersonAgeTypes.Where(x => x.PersonAgeGroup == PersonAgeGroups.Teen)
                        .Select(x => x.UnitPrice)
                        .FirstOrDefault()
                };
                return _teen;
            }
            set => _teen = value;
        }

        /// <summary>小学生低学年　★必須</summary>
        [XmlElement("teen_b")]
        [JsonProperty("teen_b")]
        public NameAndRate? TeenB
        {
            get
            {
                _teenB ??= new NameAndRate
                {
                    Count = PersonAgeTypes.Where(x => x.PersonAgeGroup == PersonAgeGroups.TeenB)
                        .Select(x => x.Number)
                        .Sum(),
                    Rate = PersonAgeTypes.Where(x => x.PersonAgeGroup == PersonAgeGroups.TeenB)
                        .Select(x => x.UnitPrice)
                        .FirstOrDefault()
                };
                return _teenB;
            }
            set => _teenB = value;
        }

        /// <summary>幼児食事あり布団あり　★必須</summary>
        [XmlElement("child_foodbed")]
        [JsonProperty("child_foodbed")]
        public NameAndRate? ChildFoodBed
        {
            get
            {
                _childFoodBed ??= new NameAndRate
                {
                    Count = PersonAgeTypes.Where(x => x.PersonAgeGroup == PersonAgeGroups.Child)
                        .Where(x => x.FoodBed == (FoodBeds.Food | FoodBeds.Bed))
                        .Select(x => x.Number)
                        .Sum(),
                    Rate = PersonAgeTypes.Where(x => x.PersonAgeGroup == PersonAgeGroups.Child)
                        .Where(x => x.FoodBed == (FoodBeds.Food | FoodBeds.Bed))
                        .Select(x => x.UnitPrice)
                        .FirstOrDefault()
                };
                return _childFoodBed;
            }
            set => _childFoodBed = value;
        }

        /// <summary>幼児食事あり布団なし　★必須</summary>
        [XmlElement("child_food")]
        [JsonProperty("child_food")]
        public NameAndRate? ChildFood
        {
            get
            {
                _childFood ??= new NameAndRate
                {
                    Count = PersonAgeTypes.Where(x => x.PersonAgeGroup == PersonAgeGroups.Child)
                        .Where(x => x.FoodBed == FoodBeds.Food)
                        .Select(x => x.Number)
                        .Sum(),
                    Rate = PersonAgeTypes.Where(x => x.PersonAgeGroup == PersonAgeGroups.Child)
                        .Where(x => x.FoodBed == FoodBeds.Food)
                        .Select(x => x.UnitPrice)
                        .FirstOrDefault()
                };
                return _childFood;
            }
            set => _childFood = value;
        }

        /// <summary>幼児食事なし布団あり　★必須</summary>
        [XmlElement("child_bed")]
        [JsonProperty("child_bed")]
        public NameAndRate? ChildBed
        {
            get
            {
                _childBed ??= new NameAndRate
                {
                    Count = PersonAgeTypes.Where(x => x.PersonAgeGroup == PersonAgeGroups.Child)
                        .Where(x => x.FoodBed == FoodBeds.Bed)
                        .Select(x => x.Number)
                        .Sum(),
                    Rate = PersonAgeTypes.Where(x => x.PersonAgeGroup == PersonAgeGroups.Child)
                        .Where(x => x.FoodBed == FoodBeds.Bed)
                        .Select(x => x.UnitPrice)
                        .FirstOrDefault()
                };
                return _childBed;
            }
            set => _childBed = value;
        }

        /// <summary>幼児食事なし布団なし　★必須</summary>
        [XmlElement("child_neither")]
        [JsonProperty("child_neither")]
        public NameAndRate? ChildNeither
        {
            get
            {
                _childNeither ??= new NameAndRate
                {
                    Count = PersonAgeTypes.Where(x => x.PersonAgeGroup == PersonAgeGroups.Child)
                        .Where(x => x.FoodBed == FoodBeds.None)
                        .Select(x => x.Number)
                        .Sum(),
                    Rate = PersonAgeTypes.Where(x => x.PersonAgeGroup == PersonAgeGroups.Child)
                        .Where(x => x.FoodBed == FoodBeds.None)
                        .Select(x => x.UnitPrice)
                        .FirstOrDefault()
                };
                return _childNeither;
            }
            set => _childNeither = value;
        }

        /// <summary>乳幼児食事あり布団あり　★必須</summary>
        [XmlElement("baby_foodbed")]
        [JsonProperty("baby_foodbed")]
        public NameAndRate? BabyFoodBed
        {
            get
            {
                _babyFoodBed ??= new NameAndRate
                {
                    Count = PersonAgeTypes.Where(x => x.PersonAgeGroup == PersonAgeGroups.Baby)
                        .Where(x => x.FoodBed == (FoodBeds.Food | FoodBeds.Bed))
                        .Select(x => x.Number)
                        .Sum(),
                    Rate = PersonAgeTypes.Where(x => x.PersonAgeGroup == PersonAgeGroups.Baby)
                        .Where(x => x.FoodBed == (FoodBeds.Food | FoodBeds.Bed))
                        .Select(x => x.UnitPrice)
                        .FirstOrDefault()
                };
                return _babyFoodBed;
            }
            set => _babyFoodBed = value;
        }

        /// <summary>乳幼児食事あり布団なし　★必須</summary>
        [XmlElement("baby_food")]
        [JsonProperty("baby_food")]
        public NameAndRate? BabyFood
        {
            get
            {
                _babyFood ??= new NameAndRate
                {
                    Count = PersonAgeTypes.Where(x => x.PersonAgeGroup == PersonAgeGroups.Baby)
                        .Where(x => x.FoodBed == FoodBeds.Food)
                        .Select(x => x.Number)
                        .Sum(),
                    Rate = PersonAgeTypes.Where(x => x.PersonAgeGroup == PersonAgeGroups.Baby)
                        .Where(x => x.FoodBed == FoodBeds.Food)
                        .Select(x => x.UnitPrice)
                        .FirstOrDefault()
                };
                return _babyFood;
            }
            set => _babyFood = value;
        }

        /// <summary>乳幼児食事なし布団あり　★必須</summary>
        [XmlElement("baby_bed")]
        [JsonProperty("baby_bed")]
        public NameAndRate? BabyBed
        {
            get
            {
                _babyBed ??= new NameAndRate
                {
                    Count = PersonAgeTypes.Where(x => x.PersonAgeGroup == PersonAgeGroups.Baby)
                        .Where(x => x.FoodBed == FoodBeds.Bed)
                        .Select(x => x.Number)
                        .Sum(),
                    Rate = PersonAgeTypes.Where(x => x.PersonAgeGroup == PersonAgeGroups.Baby)
                        .Where(x => x.FoodBed == FoodBeds.Bed)
                        .Select(x => x.UnitPrice)
                        .FirstOrDefault()
                };
                return _babyBed;
            }
            set => _babyBed = value;
        }

        /// <summary>乳幼児食事なし布団なし　★必須</summary>
        [XmlElement("baby_neither")]
        [JsonProperty("baby_neither")]
        public NameAndRate? BabyNeither
        {
            get
            {
                _babyNeither ??= new NameAndRate
                {
                    Count = PersonAgeTypes.Where(x => x.PersonAgeGroup == PersonAgeGroups.Baby)
                        .Where(x => x.FoodBed == FoodBeds.None)
                        .Select(x => x.Number)
                        .Sum(),
                    Rate = PersonAgeTypes.Where(x => x.PersonAgeGroup == PersonAgeGroups.Baby)
                        .Where(x => x.FoodBed == FoodBeds.None)
                        .Select(x => x.UnitPrice)
                        .FirstOrDefault()
                };
                return _babyNeither;
            }
            set => _babyNeither = value;
        }

        /// <summary>大人入湯税　★必須</summary>
        [XmlElement("bathtax_adult")]
        [JsonProperty("bathtax_adult")]
        public NameAndRate? BathtaxAdult
        {
            get
            {
                _bathtaxAdult ??= new NameAndRate
                {
                    Count = PersonAgeTypes.Where(x => x.PersonAgeGroup == PersonAgeGroups.Adult)
                        .Select(x => x.Number)
                        .Sum(),
                    Rate = PersonAgeTypes.Where(x => x.PersonAgeGroup == PersonAgeGroups.Adult)
                        .Select(x => x.SpaTax)
                        .FirstOrDefault()
                };
                return _bathtaxAdult;
            }
            set => _bathtaxAdult = value;
        }

        /// <summary>子供入湯税　★必須</summary>
        [XmlElement("bathtax_child")]
        [JsonProperty("bathtax_child")]
        public NameAndRate? BathtaxChild
        {
            get
            {
                _bathtaxChild ??= new NameAndRate
                {
                    Count = PersonAgeTypes.Where(x => x.PersonAgeGroup == PersonAgeGroups.Child)
                        .Select(x => x.Number)
                        .Sum(),
                    Rate = PersonAgeTypes.Where(x => x.PersonAgeGroup == PersonAgeGroups.Child)
                        .Select(x => x.SpaTax)
                        .FirstOrDefault()
                };
                return _bathtaxChild;
            }
            set => _bathtaxChild = value;
        }

        /// <summary>子供入湯税　★必須</summary>
        [XmlElement("bathtax_teen")]
        [JsonProperty("bathtax_teen")]
        public NameAndRate? BathtaxTeen
        {
            get
            {
                _bathtaxTeen ??= new NameAndRate
                {
                    Count = PersonAgeTypes
                        .Where(x => x.PersonAgeGroup is PersonAgeGroups.Teen or PersonAgeGroups.TeenB)
                        .Select(x => x.Number)
                        .Sum(),
                    Rate = PersonAgeTypes
                        .Where(x => x.PersonAgeGroup is PersonAgeGroups.Teen or PersonAgeGroups.TeenB)
                        .Select(x => x.SpaTax)
                        .FirstOrDefault()
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

        public List<RoomItem> GetRoomsGroupedByPrice()
        {
            if (
                BookingDateId is null || BookingData?.AppDates is null || PlanType is RateTypeLabels.PerPerson
            )
            {
                return [];
            }

            var rooms = BookingData.AppDates.Find(
                    ad => ad.AppDateId == BookingDateId
                )
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
