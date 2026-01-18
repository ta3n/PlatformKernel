using Liberty.Reservation.Employee.WebAPI.Application.ExternalServices.Membership.Facility.Models.Base;
using Microsoft.EntityFrameworkCore;

namespace Liberty.Reservation.Employee.WebAPI.Application.ExternalServices.Membership.Facility.Models;

public class Facility : BaseModel
{
    /// <summary>施設名</summary>
    [Comment("施設名")]
    public string? Name { get; set; }

    /// <summary>会員仮名</summary>
    [Comment("会員仮名")]
    public string? Kana { get; set; }

    /// <summary>メールアドレス</summary>
    [Comment("メールアドレス")]
    public string? Email { get; set; }

    /// <summary>住所情報情報のリレーションID</summary>
    [Comment("AddressId")]
    public long? AddressId { get; set; }

    /// <summary>住所情報</summary>
    [Comment("住所情報")]
    public Address? Address { get; set; }

    /// <summary>状態</summary>
    [Comment("状態")]
    public FacilityStates State { get; set; }
}

[Flags]
public enum FacilityStates
{
    Default = 1 << 0,
    Disabled = 1 << 1,
    Deleted = 1 << 2,
    Public = 1 << 3
}
