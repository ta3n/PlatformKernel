using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests;

[XmlRoot("request")]
public class UpdatePublishAcceptPlanRequest
{
    [JsonProperty(nameof(ScAgtPlanCode))]
    [XmlElement(nameof(ScAgtPlanCode))]
    public string? ScAgtPlanCode { get; set; }

    [JsonProperty(nameof(ScAgtFacilityCode))]
    [XmlElement(nameof(ScAgtFacilityCode))]
    public string? ScAgtFacilityCode { get; set; }

    [XmlIgnore]
    [JsonIgnore]
    public string? UseBookingReception { get; set; } = true.ToString();

    [JsonProperty(nameof(BookingReceptionStart))]
    [XmlElement(nameof(BookingReceptionStart))]
    public string? BookingReceptionStart { get; set; }

    [JsonProperty(nameof(BookingReceptionEnd))]
    [XmlElement(nameof(BookingReceptionEnd))]
    public string? BookingReceptionEnd { get; set; }

    [JsonProperty(nameof(UseDisplayDate))]
    [XmlElement(nameof(UseDisplayDate))]
    public string? UseDisplayDate { get; set; }

    [JsonProperty(nameof(DisplayDateStart))]
    [XmlElement(nameof(DisplayDateStart))]
    public string? DisplayDateStart { get; set; }

    [JsonProperty(nameof(DisplayDateEnd))]
    [XmlElement(nameof(DisplayDateEnd))]
    public string? DisplayDateEnd { get; set; }

    [JsonProperty(nameof(UseAcceptDate))]
    [XmlElement(nameof(UseAcceptDate))]
    public string? UseAcceptDate { get; set; }

    [JsonProperty(nameof(AcceptDateStart))]
    [XmlElement(nameof(AcceptDateStart))]
    public string? AcceptDateStart { get; set; }

    [JsonProperty(nameof(AcceptDateEnd))]
    [XmlElement(nameof(AcceptDateEnd))]
    public string? AcceptDateEnd { get; set; }

    [JsonProperty(nameof(ReceptionDayLimit))]
    [XmlElement(nameof(ReceptionDayLimit))]
    public string? ReceptionDayLimit { get; set; }

    [JsonProperty(nameof(ReceptionLimit))]
    [XmlElement(nameof(ReceptionLimit))]
    public string? ReceptionLimit { get; set; }
}
