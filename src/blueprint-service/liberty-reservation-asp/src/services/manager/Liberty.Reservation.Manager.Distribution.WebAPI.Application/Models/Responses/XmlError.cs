using System.Xml.Serialization;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Responses;

[XmlRoot("error_response")]
public class XmlError
{
    [XmlElement("response")]
    public BaseDataResponse<object>? Response { get; set; }
}
