using System.Xml.Serialization;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests;
using Newtonsoft.Json;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Responses;

public class GetPriceDataPlanRoomResponse
{
    [XmlElement(nameof(TariffData))]
    [JsonProperty(nameof(TariffData))]
    public List<TariffData> TariffData { get; set; } = [];
}

public class TariffData
{
    [XmlArray(nameof(Prices))]
    [XmlArrayItem("PriceDataPlanRoom")]
    [JsonProperty(nameof(Prices))]
    public List<PriceDataPlanRoom> Prices { get; set; } = [];
}

public class PriceDataPlanRoom
{
    [XmlElement(nameof(Date))]
    [JsonProperty(nameof(Date))]
    public long Date { get; set; }

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

public enum SalesStatusPriceData
{
    /// <summary>
    /// Currently on sale.
    /// </summary>
    OnSale = 0,

    /// <summary>
    /// Sold but has been stopped.
    /// </summary>
    NotSale = 1
}
