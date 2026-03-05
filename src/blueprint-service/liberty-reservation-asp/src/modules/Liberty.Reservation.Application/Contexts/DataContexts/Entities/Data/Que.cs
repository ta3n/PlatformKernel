using Liberty.Entity;
using Liberty.Reservation.Application.Constants;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

/// <summary>
/// キュー処理履歴
/// </summary>
public class Que : EntityData
{
    /// <summary>
    /// 種別
    /// </summary>
    public QueTypes QueType { get; set; }

    /// <summary>
    /// 処理内容JSON
    /// </summary>
    public string? Data { get; set; }
}
