using System.Xml.Serialization;
using Liberty.ApplicationShared.Utils;
using static Liberty.Reservation.Application.Models.Responses.SetRoomsResponse;
using Newtonsoft.Json;
using Liberty.Reservation.Application.Models.Responses;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests;

[XmlRoot("request")]
public class GetBookingRequest
{
    /// <summary>取得条件タイプ　★必須</summary>
    [XmlElement("data_type")]
    [JsonProperty("data_type")]
    public EnumDataType? DateType { get; set; }

    /// <summary>終了日時　★必須</summary>
    [XmlElement("fromday")]
    [JsonProperty("fromday")]
    public required string ConvertOnlyFromDay { get; set; }

    [XmlElement("today")]
    [JsonProperty("today")]
    public required string ConvertOnlyToDay { get; set; }

    [XmlElement("from_arriveday")]
    [JsonProperty("from_arriveday")]
    public string? ConvertOnlyFromArriveDay { get; set; }

    [XmlElement("to_arriveday")]
    [JsonProperty("to_arriveday")]
    public string? ConvertOnlyToArriveDay { get; set; }

    public class C004InvalidParameter
    {
        public FailureReason Message { get; set; }

        public HotelModel? Hotel { get; set; }
    }

    /// <summary>施設番号</summary>
    [XmlElement("hotel_id")]
    [JsonProperty("hotel_id")]
    public List<string> HotelIds { get; set; } = [];

    public DateTime GetFromDay()
    {
        return ConvertUtil.ConvertDateTime(ConvertOnlyFromDay);
    }

    public DateTime GetToDay()
    {
        return ConvertUtil.ConvertDateTime(ConvertOnlyToDay);
    }

    public DateTime? GetFromArriveDay()
    {
        return ConvertOnlyFromArriveDay != null
            ? ConvertUtil.ToDateTimeWithFormat(ConvertOnlyFromArriveDay, "yyyy-MM-dd HH:mm:ss")
            : null;
    }

    public DateTime? GetToArriveDay()
    {
        return ConvertOnlyToArriveDay != null
            ? ConvertUtil.ToDateTimeWithFormat(ConvertOnlyToArriveDay, "yyyy-MM-dd HH:mm:ss")
            : null;
    }

    public enum EnumDataType
    {
        [XmlEnum("0")]
        [JsonProperty("0")]
        ReserveOrCancel = 0,

        /// <summary>チェックインのみ(キャンセル、NoShow除く)</summary>
        [XmlEnum("1")]
        [JsonProperty("1")]
        CheckIn = 1,

        /// <summary>予約受付のみ(キャンセル除く、NoShow)</summary>
        [XmlEnum("2")]
        [JsonProperty("2")]
        Reserve = 2,

        /// <summary>キャンセル受付のみ</summary>
        [XmlEnum("3")]
        [JsonProperty("3")]
        Cancel = 3,

        /// <summary>NoShow受付のみ</summary>
        [XmlEnum("4")]
        [JsonProperty("4")]
        NoShow = 4
    }
}
