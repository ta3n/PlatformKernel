using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests;

[XmlRoot("request")]
public class GetRoomsRequest
{
    /// <summary>開始日 string(yyyy-MM-dd)</summary>
    [XmlElement("fromday", DataType = "date")]
    [JsonProperty("fromday")]
    public required DateTime FromDay { get; set; }

    ///// <summary>終了日 string(yyyy-MM-dd)</summary>
    [XmlElement("today", DataType = "date")]
    [JsonProperty("today")]
    public required DateTime ToDay { get; set; }

    /// <summary>施設番号リスト MaxCount=10</summary>
    [XmlElement("hotel_id")]
    [JsonProperty("hotel_id")]
    public required List<string> HotelIds { get; set; }

    [XmlIgnore]
    [JsonIgnore]
    public long? FromAppDateId => AppDate.GetId(FromDay);

    [XmlIgnore]
    [JsonIgnore]
    public long? ToAppDateId => AppDate.GetId(ToDay);
}
