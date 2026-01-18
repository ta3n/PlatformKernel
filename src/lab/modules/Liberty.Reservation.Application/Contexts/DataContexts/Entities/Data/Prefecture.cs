using Liberty.Entity;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

/// <summary>
/// 都道府県
/// </summary>
public class Prefecture : EntityData
{
    /// <summary>
    /// 名称
    /// </summary>
    public string? Name { get; set; }
}
