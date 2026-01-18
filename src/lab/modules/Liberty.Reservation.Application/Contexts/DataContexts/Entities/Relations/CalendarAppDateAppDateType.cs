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

    public long AppDateId { get; set; }

    public AppDate? AppDate { get; set; }

    public long AppDateTypeId { get; set; }

    public AppDateType? AppDateType { get; set; }

    public CalendarAppDateAppDateType()
    {
    }

    public CalendarAppDateAppDateType(
        Calendar calendar,
        AppDate appDate,
        AppDateType appDateType
    )
    {
        CalendarId = calendar.Id;
        Calendar = calendar;
        AppDateId = appDate.Id;
        AppDate = appDate;
        AppDateTypeId = appDateType.Id;
        AppDateType = appDateType;
    }
}
