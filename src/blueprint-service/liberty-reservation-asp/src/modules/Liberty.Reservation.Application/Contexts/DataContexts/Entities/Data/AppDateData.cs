using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

public class AppDateData : EntityData
{
    public string? Name { get; set; }
    public string? Color { get; set; }

    /// <summary>
    /// マスタ日付情報データリレーション
    /// </summary>
    public ICollection<AppDateAppDateData>? AppDateAppDateData { get; set; }
}
