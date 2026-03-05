using Liberty.Entity;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

/// <summary>
/// 消費税
/// </summary>
public class ConsumptionTax : EntityData
{
    public string? Name { get; set; }

    public float Rate { get; set; }

    /// <summary>
    /// 運用開始日時
    /// </summary>
    public long EnabledStart { get; set; }

    /// <summary>
    /// 運用終了日時
    /// </summary>
    public long EnabledEnd { get; set; }
}
