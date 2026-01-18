using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Responses;

[XmlRoot("response")]
public class RoomAdjustmentStatusResponse
{
    [XmlAttribute("total_count")]
    [JsonProperty("total_count")]
    public int TotalCount { get; set; }

    [XmlAttribute("success_count")]
    [JsonProperty("success_count")]
    public int SuccessCount { get; set; }

    [XmlAttribute("error_count")]
    [JsonProperty("error_count")]
    public int ErrorCount { get; set; }

    [XmlElement("hotel_message")]
    [JsonProperty("hotel_message")]
    public List<AdjustmentResultResponse> AdjustmentResults { get; set; } = [];
}

public class AdjustmentResultResponse
{
    [XmlAttribute("hotel_id")]
    [JsonProperty("hotel_id")]
    public string HotelId { get; set; } = string.Empty;

    [XmlAttribute("room_id")]
    [JsonProperty("room_id")]
    public string RoomId { get; set; } = string.Empty;

    [XmlAttribute("reason")]
    [JsonProperty("reason")]
    public string? Reason { get; set; }

    [XmlAttribute("is_success")]
    [JsonProperty("is_success")]
    public bool IsSuccess { get; set; }
}
