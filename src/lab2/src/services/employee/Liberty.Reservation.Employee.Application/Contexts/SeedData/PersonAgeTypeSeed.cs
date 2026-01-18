using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;

namespace Liberty.Reservation.Employee.Application.Contexts.SeedData;

public class PersonAgeTypeSeed
{
    public string? Code { get; set; }

    public string? Name { get; set; }

    public bool IsMaster { get; set; }

    /// <summary>
    /// 大人〇名として料金指定の際のメインとなる区分としてあつかうか？
    /// </summary>
    public bool IsMain { get; set; }

    /// <summary>
    /// 年齢上限
    /// </summary>
    public int? AgeMax { get; set; }

    /// <summary>
    /// 年齢下限
    /// </summary>
    public int? AgeMin { get; set; }

    /// <summary>
    /// 情報JSON
    /// </summary>
    public PersonAgeTypeMeta? Meta { get; set; }

    public List<SpaTaxData> SpaTaxes { get; set; } = [];
}
