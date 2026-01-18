using System.Xml.Serialization;
using Liberty.Reservation.Application.Models.Responses;
using Newtonsoft.Json;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests;

[XmlRoot("request")]
public class SetRoomsRequest
{
    [XmlElement("hotel")]
    [JsonProperty("hotel")]
    public List<HotelModel> Hotels { get; set; } = [];
}
