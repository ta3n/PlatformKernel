using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

/// <summary>
/// 施設が作成する料金カレンダー
/// </summary>
public class Calendar : EntityData
{
    /// <summary>
    /// カレンダー名
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 説明
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// イベントメモ
    /// </summary>
    public string? EventMemo { get; set; }

    /// <summary>
    /// 施設カレンダーリレーション
    /// </summary>
    public ICollection<FacilityCalendar>? FacilityCalendars { get; set; }

    public ICollection<CalendarAppDateAppDateType>? CalendarAppDateAppDateTypes { get; set; }
}
