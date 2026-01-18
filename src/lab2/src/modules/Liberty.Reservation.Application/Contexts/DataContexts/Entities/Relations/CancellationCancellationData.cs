using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

/// <summary>
/// キャンセル規約ーキャンセル料情報リレーション
/// </summary>
public class CancellationCancellationData : EntityRelation
{
    public long CancellationId { get; set; }
    public Cancellation? Cancellation { get; set; }

    public long CancellationDataId { get; set; }
    public CancellationData? CancellationData { get; set; }
}
