using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Responses;

public class GetPlanResponse
{
    [XmlArray("AgtPlanRoomInfos")]
    [XmlArrayItem("AgtPlanRoomInfos")]
    [JsonProperty(nameof(AgtPlanRoomInfos))]
    public List<PlanRoomGroupDto> AgtPlanRoomInfos { get; set; } = [];
}

public class PlanRoomPriceData
{
    [XmlElement(nameof(PersonMax))]
    [JsonProperty(nameof(PersonMax))]
    public string? PersonMax { get; set; }

    [XmlElement(nameof(PersonMin))]
    [JsonProperty(nameof(PersonMin))]
    public string? PersonMin { get; set; }

    [XmlElement(nameof(Price))]
    [JsonProperty(nameof(Price))]
    public string? Price { get; set; }
}

public class PlanRoomGroupDto
{
    [XmlElement(nameof(ScAgtPlanCode))]
    [JsonProperty(nameof(ScAgtPlanCode))]
    public string? ScAgtPlanCode { get; set; }

    [XmlElement(nameof(ScAgtRoomCode))]
    [JsonProperty(nameof(ScAgtRoomCode))]
    public string? ScAgtRoomCode { get; set; }

    [XmlElement(nameof(PlanIndicationName))]
    [JsonProperty(nameof(PlanIndicationName))]
    public string? PlanIndicationName { get; set; }

    [XmlElement(nameof(ReserveInformationEndDay))]
    [JsonProperty(nameof(ReserveInformationEndDay))]
    public int ReserveInformationEndDay { get; set; }

    [XmlElement(nameof(ReserveInformationEndTime))]
    [JsonProperty(nameof(ReserveInformationEndTime))]
    public string? ReserveInformationEndTime { get; set; }

    [XmlElement(nameof(EnforcementPeriodFrom))]
    [JsonProperty(nameof(EnforcementPeriodFrom))]
    public string? EnforcementPeriodFrom { get; set; }

    [XmlElement(nameof(EnforcementPeriodTo))]
    [JsonProperty(nameof(EnforcementPeriodTo))]
    public string? EnforcementPeriodTo { get; set; }

    [XmlIgnore]
    [JsonIgnore]
    public long DisplayOrder { get; set; }
}

public enum SalesStatus
{
    /// <summary>
    /// Currently on sale.
    /// </summary>
    OnSale = 0,

    /// <summary>
    /// Sold but has been stopped.
    /// </summary>
    SalesStopped = 1,

    /// <summary>
    /// Never sold.
    /// </summary>
    NotYetSold = 2,

    /// <summary>
    /// Sales ended. Unused item.
    /// </summary>
    SalesEnded = 3
}
