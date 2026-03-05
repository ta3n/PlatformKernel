using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Responses;

public class UpdatePublishAcceptPlanResponse
{
    [JsonProperty(nameof(PlanId))]
    [XmlElement("ScAgtPlanCode")]
    public required string PlanId { get; init; }

    [JsonProperty(nameof(UseBookingReception))]
    [XmlElement("UseBookingReception")]
    public bool? UseBookingReception { get; set; }

    [JsonProperty(nameof(BookingReceptionStart))]
    [XmlElement("BookingReceptionStart")]
    public string? BookingReceptionStart { get; set; }

    [JsonProperty(nameof(BookingReceptionEnd))]
    [XmlElement("BookingReceptionEnd")]
    public string? BookingReceptionEnd { get; set; }

    [JsonProperty(nameof(UseDisplayDate))]
    [XmlElement("UseDisplayDate")]
    public bool? UseDisplayDate { get; set; }

    [JsonProperty(nameof(DisplayDateStart))]
    [XmlElement("DisplayDateStart")]
    public string? DisplayDateStart { get; set; }

    [JsonProperty(nameof(DisplayDateEnd))]
    [XmlElement("DisplayDateEnd")]
    public string? DisplayDateEnd { get; set; }

    [JsonProperty(nameof(UseAcceptDate))]
    [XmlElement("UseAcceptDate")]
    public bool? UseAcceptDate { get; set; }

    [JsonProperty(nameof(AcceptDateStart))]
    [XmlElement("AcceptDateStart")]
    public string? AcceptDateStart { get; set; }

    [JsonProperty(nameof(AcceptDateEnd))]
    [XmlElement("AcceptDateEnd")]
    public string? AcceptDateEnd { get; set; }

    [JsonProperty(nameof(ReceptionDayLimit))]
    [XmlElement("ReceptionDayLimit")]
    public string? ReceptionDayLimit { get; set; }

    [JsonProperty(nameof(ReceptionLimit))]
    [XmlElement("ReceptionLimit")]
    public string? ReceptionLimit { get; set; }
}
