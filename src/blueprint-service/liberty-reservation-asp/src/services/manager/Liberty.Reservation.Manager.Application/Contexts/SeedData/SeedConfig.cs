namespace Liberty.Reservation.Manager.Application.Contexts.SeedData;

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
    public string MetaJson { get; set; } = "{}";

    public List<SpaTaxOfPersonAgeTypeSeed>? SpaTaxes { get; set; }
}

public class SpaTaxOfPersonAgeTypeSeed
{
    /// <summary>
    /// 金額上限
    /// </summary>
    public int? PriceMax { get; set; }

    /// <summary>
    /// 金額下限
    /// </summary>
    public int? PriceMin { get; set; }

    /// <summary>
    /// 入湯税
    /// </summary>
    public int? Tax { get; set; }
}
