namespace Liberty.Reservation.User.File.WebAPI.Application.ExternalServices.Membership.Facility.Models.Base;

public class BaseModel
{
    /// <summary>Id</summary>
    [Comment("Id")]
    public long Id { get; set; }

    /// <summary>コード</summary>
    [Comment("コード")]
    public required string Code { get; set; }

    [Comment("レコードメモ")]
    public string? Memo { get; set; }
}
