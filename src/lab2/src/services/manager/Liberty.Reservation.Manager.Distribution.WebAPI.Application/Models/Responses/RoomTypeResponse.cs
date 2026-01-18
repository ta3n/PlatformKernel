using System.Xml.Serialization;
using Liberty.SysException;
using Newtonsoft.Json;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Responses;

[XmlRoot("response")]
public class RoomTypeResponse
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
}

public class Hotel
{
    [XmlAttribute("id")]
    [JsonProperty("id")]
    public string? Id { get; set; }

    [XmlElement("room")]
    [JsonProperty("room")]
    public List<RoomResponse> Rooms { get; set; } = [];
}

public class RoomResponse
{
    [XmlAttribute("room_id")]
    [JsonProperty("room_id")]
    public string? RoomId { get; set; }

    [XmlAttribute("name")]
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [XmlAttribute("rooms")]
    [JsonProperty("rooms")]
    public long Rooms { get; set; }

    [XmlAttribute("people")]
    [JsonProperty("people")]
    public long People { get; set; }
}
