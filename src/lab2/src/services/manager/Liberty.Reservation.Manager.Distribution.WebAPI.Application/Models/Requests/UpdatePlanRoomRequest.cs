using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests;

[XmlRoot("request")]
public class UpdatePlanRoomRequest
{
    [XmlElement(nameof(ScAgtFacilityCode))]
    [JsonProperty(nameof(ScAgtFacilityCode))]
    public string? ScAgtFacilityCode { get; set; }

    [XmlElement(nameof(ScAgtPlanCode))]
    [JsonProperty(nameof(ScAgtPlanCode))]
    public string? ScAgtPlanCode { get; set; }

    [XmlElement(nameof(ScAgtRoomCode))]
    [JsonProperty(nameof(ScAgtRoomCode))]
    public string? ScAgtRoomCode { get; set; }

    [XmlElement(nameof(ScAgtSiteCode))]
    [JsonProperty(nameof(ScAgtSiteCode))]
    public string? ScAgtSiteCode { get; set; }

    [XmlArray("PriceData")]
    [XmlArrayItem("PriceItem")]
    [JsonProperty(nameof(PriceData))]
    public List<UpdatePriceData>? PriceData { get; set; }
}

public class UpdatePriceData
{
    [XmlElement("AppointedDate")]
    [JsonProperty(nameof(AppointedDate))]
    public string? AppointedDate { get; set; }

    [XmlElement("StopStartDivision")]
    [JsonProperty(nameof(StopStartDivision))]
    public string? StopStartDivision { get; set; }

    [XmlArray("PriceElements")]
    [XmlArrayItem("PriceElement")]
    [JsonProperty(nameof(PriceElement))]
    public List<UpdatePriceDataItem>? PriceElement { get; set; }
}

public class UpdatePriceDataItem
{
    [XmlElement("PersonMax")]
    [JsonProperty(nameof(PersonMax))]
    public string? PersonMax { get; set; }

    [XmlElement("PersonMin")]
    [JsonProperty(nameof(PersonMin))]
    public string? PersonMin { get; set; }

    [XmlElement("Price")]
    [JsonProperty(nameof(Price))]
    public string? Price { get; set; }
}
