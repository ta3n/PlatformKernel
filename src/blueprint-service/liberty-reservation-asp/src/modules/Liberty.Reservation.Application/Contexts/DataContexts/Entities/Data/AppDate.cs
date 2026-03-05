using Liberty.ApplicationShared.Utils;
using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

/// <summary>
/// アプリケーションカレンダー
/// システム内で使用されるアプリケーション用カレンダー
/// </summary>
public class AppDate : EntityData
{
    public int Year => DateTime.Year;
    public int Month => DateTime.Month;

    /// <summary>
    /// 日付
    /// </summary>
    public DateTime DateTime { get; set; }

    /// <summary>
    /// マスタ日付情報データリレーション
    /// </summary>
    public ICollection<AppDateAppDateData>? AppDateAppDateDatas { get; set; }

    /// <summary>
    /// アプリ日付・日付タイプリレーション
    /// </summary>
    public ICollection<AppDateAppDateType>? AppDateAppDateTypes { get; set; }

    /// <summary>
    /// プランサイト部屋日付リレーション
    /// </summary>
    public ICollection<PlanRoomGroupSiteAppDate>? PlanRoomGroupSiteAppDates { get; set; }

    /// <summary>
    /// サイト部屋日付リレーション
    /// </summary>
    public ICollection<RoomGroupSiteAppDate>? RoomGroupSiteAppDates { get; set; }

    /// <summary>
    /// プランサイト部屋日付料金リレーション
    /// </summary>
    public ICollection<PlanRoomGroupSiteAppDatePriceData>? PlanRoomGroupSiteAppDatePriceDatas { get; set; }

    //public ICollection<CalendarAppDateAppDateType>? CalendarAppDateAppDateTypes { get; set; }

    /// <summary>
    /// 部屋ー日付リレーション
    /// 在庫情報がふくまれる
    /// </summary>
    public ICollection<RoomGroupAppDate>? RoomGroupAppDates { get; set; }

    /// <summary>
    /// オプションアイテムー日付リレーション
    /// 在庫情報がふくまれる
    /// </summary>
    public ICollection<OptionItemAppDate>? OptionItemAppDates { get; set; }

    /// <summary>
    /// 予約 プラン　・　部屋・日付リレーション
    /// </summary>
    public ICollection<ReservationPlanRoomGroupAppDate>? ReservationPlanRoomGroupAppDates { get; set; }

    public ICollection<ReservationRoomGroupAppDatePersonAgeType>? ReservationRoomGroupAppDatePersonAgeTypes
    {
        get;
        set;
    }

    public ICollection<ReservationRoomGroupAppDateOptionItem>? ReservationRoomGroupAppDateOptionItems { get; set; }

    public ICollection<RoomGroupSiteAppDatePriceData>? RoomGroupSiteAppDatePriceDatas { get; set; }

    public static long GetId(
        DateTime datetime
    )
    {
        return ConvertUtil.ToInt(ConvertUtil.ToString(datetime, "yyyyMMdd"));
    }

    public static long? GetId(
        DateTime? datetime
    )
    {
        return datetime == null ? null : GetId(datetime.Value);
    }

    /// <summary>
    /// IDが正しい形式で変換できるか試しできなければ例外
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static long? Perse(
        long? id
    )
    {
        if (id is null)
        {
            return null;
        }

        var datetime = GetDateTime(id.Value);

        return GetId(datetime);
    }

    public static DateTime GetDateTime(
        long id,
        TimeSpan? timeSpan = null
    )
    {
        var idStr = ConvertUtil.ToString(id);

        if (idStr.Length < 8)
        {
            throw new ArgumentException(nameof(idStr));
        }

        var yearStr = idStr[..4];
        var monthStr = idStr.Substring(4, 2);
        var dayStr = idStr.Substring(6, 2);

        var date = new DateTime(
            ConvertUtil.ToInt(yearStr),
            ConvertUtil.ToInt(monthStr),
            ConvertUtil.ToInt(dayStr),
            0,
            0,
            0,
            DateTimeKind.Utc
        );

        return timeSpan.HasValue ? date.Add(timeSpan.Value) : date;
    }

    public static DateTime? GetDateTime(
        long? id
    )
    {
        if (id is null)
        {
            return null;
        }

        return GetDateTime(id.Value);
    }
}
