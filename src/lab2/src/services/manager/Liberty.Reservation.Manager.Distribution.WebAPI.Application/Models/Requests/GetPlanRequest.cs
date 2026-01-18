using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests;

[XmlRoot("GetPlanRequest")]
public record GetPlanRequest
{
    [JsonProperty(nameof(ScAgtFacilityCode))]
    [XmlAttribute(nameof(ScAgtFacilityCode))]
    public string? ScAgtFacilityCode { get; init; }
}
