using Liberty.Entity;
using Microsoft.EntityFrameworkCore;

namespace Liberty.Reservation.Employee.Application.Contexts.Entities;

/// <summary>住所情報</summary>
[Comment("住所情報")]
public class Address : EntityData
{
    /// <summary>電話番号</summary>
    [Comment("電話番号")]
    public string? Tel { get; set; }

    /// <summary>Mobile番号</summary>
    [Comment("Mobile番号")]
    public string? Mobile { get; set; }
}
