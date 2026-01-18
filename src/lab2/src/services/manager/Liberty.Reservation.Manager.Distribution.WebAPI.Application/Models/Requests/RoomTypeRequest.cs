using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests;

[XmlRoot("request")]
public class RoomTypeRequest
{
    [XmlElement("hotel_id")]
    [JsonProperty("hotel_id")]
    public List<string> HotelIds { get; set; } = [];
}
