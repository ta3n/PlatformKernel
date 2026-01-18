using Liberty.ApplicationShared.Utils;
using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

public class FacilityCalendar : EntityRelation
{
    public long FacilityId { get; set; }
    public Facility? Facility { get; set; }

    public long CalendarId { get; set; }
    public Calendar? Calendar { get; set; }

    /// <summary>
    /// 施設追加時に作成された標準カレンダー
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// 運用開始日時
    /// </summary>
    public long? EnabledStart { get; set; }

    /// <summary>
    /// 運用終了日時
    /// </summary>
    public long? EnabledEnd { get; set; }

    public void CheckInDate(
        long appDateId
    )
    {
        ValidateUtil.CheckInDate(
            AppDate.GetDateTime(appDateId),
            AppDate.GetDateTime(EnabledStart),
            AppDate.GetDateTime(EnabledEnd)
        );
    }
}
