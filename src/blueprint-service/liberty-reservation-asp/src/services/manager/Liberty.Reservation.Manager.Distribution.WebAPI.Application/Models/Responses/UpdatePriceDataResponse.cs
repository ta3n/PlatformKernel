using System.Xml.Serialization;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests;
using Newtonsoft.Json;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Responses;

public class UpdatePriceDataResponse
{
    [JsonProperty(nameof(ScAgtPlanCode))]
    [XmlElement("ScAgtPlanCode")]
    public string? ScAgtPlanCode { get; init; }

    [XmlElement("ScAgtRoomCode")]
    [JsonProperty(nameof(ScAgtRoomCode))]
    public string? ScAgtRoomCode { get; set; }

    [XmlElement("ScAgtSiteCode")]
    [JsonProperty(nameof(ScAgtSiteCode))]
    public string? ScAgtSiteCode { get; set; }

    [XmlArray("PriceData")]
    [XmlArrayItem("PriceItem")]
    [JsonProperty(nameof(PriceData))]
    public List<PriceDatum>? PriceData { get; set; }
}

public class PriceDatum
{
    [XmlElement("Success")]
    [JsonProperty(nameof(Success))]
    public string? Success { get; set; } = true.ToString();

    [XmlElement("ErrorMsg")]
    [JsonProperty(nameof(ErrorMsg))]
    public string? ErrorMsg { get; set; }

    [XmlElement(nameof(AppointedDate))]
    [JsonProperty(nameof(AppointedDate))]
    public long AppointedDate { get; set; }

    [XmlElement(nameof(SaleStopState))]
    [JsonProperty(nameof(SaleStopState))]
    public int SaleStopState { get; set; }

    [XmlArray("PriceElements")]
    [XmlArrayItem("PriceElement")]
    [JsonProperty(nameof(PriceElement))]
    public List<UpdatePriceDataItem>? PriceElement { get; set; }

    [XmlElement(nameof(ClosingState))]
    [JsonProperty(nameof(ClosingState))]
    public int ClosingState { get; set; }
}
