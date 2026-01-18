using Liberty.Entity;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

public class RoomAdjustmentStatus : EntityData
{
    public int TotalCount { get; set; }

    public int SuccessCount => AdjustmentResults?.Count(x => x.IsSuccess) ?? 0;

    public int ErrorCount => AdjustmentResults?.Count(x => !x.IsSuccess) ?? 0;

    public ICollection<AdjustmentResult>? AdjustmentResults { get; set; }
}
