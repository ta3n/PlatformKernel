using Liberty.Entity;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

public class Country : EntityData
{
    public string? Zone { get; set; }
    public string? Name { get; set; }

    public string? EnglishName { get; set; }

    /// <summary>
    /// 言語コード
    /// </summary>
    public string? LangCode { get; set; }
}
