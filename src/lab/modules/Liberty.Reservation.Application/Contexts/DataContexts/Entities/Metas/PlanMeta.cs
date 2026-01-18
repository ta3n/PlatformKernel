namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;

public class PlanMeta
{
    /// <summary>
    /// 見出し1
    /// </summary>
    public string? Heading1 { get; set; }

    /// <summary>
    /// プランの概要
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// プランの詳細
    /// </summary>
    public string? Description { get; set; }

    public string? Payment { get; set; }
    public string? Meal { get; set; }
    public string? BarrierFree { get; set; }
    public string? SpaTax { get; set; }
    public string? SpaTaxTable { get; set; }
    public string? Cancelling { get; set; }
    public string? CancellingTable { get; set; }
    public string? Other { get; set; }
}
