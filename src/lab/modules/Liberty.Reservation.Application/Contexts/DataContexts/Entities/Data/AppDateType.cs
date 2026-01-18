using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

/// <summary>
///  日付分類
///   仕様書上では「料金タイプ」（システムカレンダーの料金タイプ）と呼び、日付の「祝日」や土曜日などの種別を設定する
///   日付分類の方がふさわしいため本名称で実装する
/// </summary>
public class AppDateType : EntityData
{
    /// <summary>
    ///  料金区分名称
    /// </summary>
    public string? Name { get; set; }

    public string? ShortName { get; set; }

    public string? Color { get; set; }

    public string? Description { get; set; }

    public bool IsMaster { get; set; }

    /// <summary>
    /// アプリ日付・日付タイプリレーション
    /// </summary>
    public ICollection<AppDateAppDateType>? AppDateAppDateTypes { get; set; }

    public ICollection<FacilityAppDateType>? FacilityAppDateTypes { get; set; }

    public ICollection<RoomGroupAppDateTypePriceData>? RoomGroupAppDateTypePriceData { get; set; }

    public ICollection<PlanRoomGroupSiteAppDateTypePriceData>? PlanRoomGroupSiteAppDateTypePriceData { get; set; }

    public ICollection<CalendarAppDateAppDateType>? CalendarAppDateAppDateTypes { get; set; }

    public ICollection<RoomGroupSiteAppDateTypePriceData>? RoomGroupSiteAppDateTypePriceData { get; set; }
}
