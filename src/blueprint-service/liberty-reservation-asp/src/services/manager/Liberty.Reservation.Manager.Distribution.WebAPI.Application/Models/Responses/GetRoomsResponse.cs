using System.Xml.Serialization;
using Liberty.SysException;
using Newtonsoft.Json;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Responses;

[XmlRoot("response")]
public class GetRoomsResponse
{
    [XmlElement("hotel")]
    [JsonProperty("hotel")]
    public required List<HotelDataResponse> Hotels { get; set; }

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
}

public class HotelDataResponse
{
    /// <summary>施設番号　★必須</summary>
    [XmlAttribute("id")]
    [JsonProperty("id")]
    public string? HotelId { get; set; }

    [XmlElement("room")]
    [JsonProperty("room")]
    public required List<Room> Rooms { get; set; }
}

public class Room
{
    /// <summary>部屋番号　★必須</summary>
    [XmlAttribute("id")]
    [JsonProperty("id")]
    public string? RoomId { get; set; }

    [XmlElement("date")]
    [JsonProperty("date")]
    public required List<Date> Dates { get; set; }
}

public class Date
{
    /// <summary>宿泊日　★必須</summary>
    [XmlAttribute("value")]
    [JsonProperty("value")]
    public required string TargetDate { get; set; }

    /// <summary>提供数　★必須</summary>
    [XmlAttribute("total")]
    [JsonProperty("total")]
    public long TotalRoomCount { get; set; }

    /// <summary>残室数　★必須</summary>
    [XmlAttribute("vacant")]
    [JsonProperty("vacant")]
    public long VacantCount { get; set; }

    /// <summary>販売停止　★必須</summary>
    [XmlAttribute("close")]
    [JsonProperty("close")]
    public EnumCloseType CloseType { get; set; }
}

public enum EnumCloseType
{
    [XmlEnum("0")]
    [JsonProperty("0")]
    Sale,

    [XmlEnum("1")]
    [JsonProperty("1")]
    Stop
}
