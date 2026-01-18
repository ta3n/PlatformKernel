using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Responses;

[XmlRoot("response")]
public class BaseDataResponse<T>
{
    [XmlElement("Success")]
    [JsonProperty(nameof(Success))]
    public string? Success { get; set; } = true.ToString();

    [XmlElement("ErrorMsg")]
    [JsonProperty(nameof(ErrorMsg))]
    public string? ErrorMsg { get; set; }

    [JsonProperty(nameof(Data))]
    [XmlElement("Data")]
    public T? Data { get; set; }
}
