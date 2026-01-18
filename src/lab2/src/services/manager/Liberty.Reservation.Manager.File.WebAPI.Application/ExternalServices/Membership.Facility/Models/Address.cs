using Liberty.Reservation.Manager.File.WebAPI.Application.ExternalServices.Membership.Facility.Models.Base;

namespace Liberty.Reservation.Manager.File.WebAPI.Application.ExternalServices.Membership.Facility.Models;

public class Address : BaseModel
{
    /// <summary>電話番号</summary>
    [Comment("電話番号")]
    public string? Tel { get; set; }

    /// <summary>Mobile番号</summary>
    [Comment("Mobile番号")]
    public string? Mobile { get; set; }

    /// <summary>郵便番号</summary>
    [Comment("郵便番号")]
    public string? Postcode { get; set; }

    /// <summary>国コード</summary>
    [Comment("国コード")]
    public string? CountryCode { get; set; }

    /// <summary>Address1</summary>
    [Comment("Address1")]
    public string? Address1 { get; set; }

    /// <summary>Address2</summary>
    [Comment("Address2")]
    public string? Address2 { get; set; }

    /// <summary>Address3</summary>
    [Comment("Address3")]
    public string? Address3 { get; set; }

    /// <summary>Address4</summary>
    [Comment("Address4")]
    public string? Address4 { get; set; }
}
