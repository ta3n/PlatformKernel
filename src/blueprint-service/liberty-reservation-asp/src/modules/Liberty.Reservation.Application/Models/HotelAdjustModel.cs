using Liberty.ApplicationShared.Utils;
using Liberty.Reservation.Application.Constants;

namespace Liberty.Reservation.Application.Models;

public class HotelAdjustModel
{
    public string? HotelId { get; set; }

    public string? RoomId { get; set; }

    public EnumAdjustType AdjustType { get; set; } = EnumAdjustType.None;

    public long RoomCount { get; set; }

    public string FromDay { get; set; } = string.Empty;

    public long Days { get; set; }

    public string Dates { get; set; } = string.Empty;

    public DateTime GetDateTimeFromDay()
    {
        return ConvertUtil.ToDateTime(
            FromDay,
            RegexValues.DateFormat
        );
    }

    public List<DateTime> GetDateTimeDates()
    {
        return ConvertUtil.ConvertDateTime(
            Dates,
            RegexValues.DateFormat
        );
    }

    public EnumDateType GetDateType()
    {
        if (string.IsNullOrEmpty(FromDay))
        {
            return string.IsNullOrEmpty(Dates) ? EnumDateType.NoUse : EnumDateType.UseDates;
        }

        return !string.IsNullOrEmpty(Dates) ? EnumDateType.UseAll : EnumDateType.UseFromDay;
    }

    public List<long> GetDates()
    {
        var dateType = GetDateType();

        switch (dateType)
        {
            case EnumDateType.UseFromDay:
                // Datesが1件未満の場合、FromDayからDays数分のリストを作成
                // NOTE: "Enumerable.Range(開始値, ループの処理回数)"
                var fromDayDateTime = ConvertUtil.ToDateTime(FromDay);
                return [.. Enumerable.Range(0, (int)Days).Select(i => ConvertUtil.ToLong(fromDayDateTime.AddDays(i)))];
            case EnumDateType.UseDates:
                // NOTE: string hotel.Dates("'日付','日付',...") を分割＆変換
                return [.. Dates.Split(',').Select(a => ConvertUtil.ToLong(ConvertUtil.ToDateTime(a[1..^1], RegexValues.DateFormat)))];
            default:
                return [];
        }
    }

    /// <summary>
    /// 日付パラメータ指定タイプ
    /// </summary>
    public enum EnumDateType
    {
        /// <summary>日付パラメータ未使用</summary>
        NoUse,

        /// <summary>日付パラメータ両方使用</summary>
        UseAll,

        /// <summary>FromDay使用</summary>
        UseFromDay,

        /// <summary>Dates使用</summary>
        UseDates
    }

    public enum EnumAdjustType
    {
        None = 0,

        RelativeDown = 1,

        RelativeUp = 2,

        Absolute = 3,

        Stop = 4,

        Sale = 5
    }
}
