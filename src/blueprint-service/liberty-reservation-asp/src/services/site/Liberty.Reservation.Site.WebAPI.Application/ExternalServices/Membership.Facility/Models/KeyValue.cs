using Liberty.Reservation.Site.WebAPI.Application.ExternalServices.Membership.Facility.Models.Base;

namespace Liberty.Reservation.Site.WebAPI.Application.ExternalServices.Membership.Facility.Models;

public class KeyValue : BaseModel
{
    public string? Record { get; set; }
    public string? Key { get; set; }
    public string? Value { get; set; }
}
