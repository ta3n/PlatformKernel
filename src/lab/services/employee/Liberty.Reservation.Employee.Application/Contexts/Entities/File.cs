using Liberty.Entity;
using Liberty.Reservation.Employee.Application.Contexts.Entities.Relations;
using Microsoft.EntityFrameworkCore;

namespace Liberty.Reservation.Employee.Application.Contexts.Entities;

[Comment("ファイル")]
public class File : EntityData
{
    /// <summary>シークレット</summary>
    [Comment("シークレット")]
    public string? Secret { get; set; }

    [Comment("社員とのリレーション")]
    public ICollection<FileEmployee>? FileEmployees { get; set; }
}
