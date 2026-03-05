namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;

public class PlanMeta
{
    /// <summary>
    /// 見出し1
    /// </summary>
    public string? Heading1 { get; set; }

    public string? Description { get; set; }
    public string? BarrierFree { get; set; }
    public string? SpaTax { get; set; }
    public string? SpaTaxTable { get; set; }
    public string? Cancelling { get; set; }
    public string? CancellingTable { get; set; }
}
