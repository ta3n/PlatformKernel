using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

public class CalendarAppDateAppDateType : EntityRelation
{
    /// <summary>
    /// 料金カレンダーID
    /// </summary>
    public long CalendarId { get; set; }

    /// <summary>
    /// 料金カレンダー
    /// </summary>
    public Calendar? Calendar { get; set; }

    public long AppDateTypeId { get; set; }

    public AppDateType? AppDateType { get; set; }

    public int DateCalendar { get; set; }
}
