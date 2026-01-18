using Liberty.Entity;
using Liberty.Reservation.Employee.Application.Contexts.Entities.Metas;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Liberty.Reservation.Employee.Application.Contexts.Entities;

[Comment("リバティー社員メタ情報")]
public class EmployeeMeta : EntityData
{
    /// <summary>社員名</summary>
    [Comment("社員名")]
    public string? Name { get; set; }

    /// <summary>社員仮名</summary>
    [Comment("社員仮名")]
    public string? Kana { get; set; }

    [Comment("性別 ※定義できない程種別があるため")]
    public string? Gender { get; set; }

    [Comment("誕生日")]
    public DateTime? Birthday { get; set; }

    [Comment("一時的に使用する領域 ")]
    public string TemporarilyJson { get; set; } = "{}";

    public EmployeeTemporarily? Temporarily
    {
        get => string.IsNullOrEmpty(TemporarilyJson)
            ? new EmployeeTemporarily()
            : JsonConvert.DeserializeObject<EmployeeTemporarily>(TemporarilyJson);
        set => TemporarilyJson = JsonConvert.SerializeObject(value);
    }
}
