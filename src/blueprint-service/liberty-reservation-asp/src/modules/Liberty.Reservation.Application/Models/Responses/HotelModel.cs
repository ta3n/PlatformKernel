using System.Text.Json.Serialization;
using System.Xml.Serialization;
using Liberty.ApplicationShared.Utils;
using Liberty.Reservation.Application.Constants;

namespace Liberty.Reservation.Application.Models.Responses;

[XmlRoot("response")]
public class HotelModel
{
    /// <summary>requestとresponseの紐づけkey</summary>
    [XmlIgnore]
    [JsonIgnore]
    public int Key { get; set; }

    /// <summary>施設番号</summary>
    [XmlAttribute("hotel_id")]
    [JsonPropertyName("hotel_id")]
    public string? HotelId { get; set; }

    /// <summary>部屋番号</summary>
    [XmlAttribute("room_id")]
    [JsonPropertyName("room_id")]
    public string? RoomId { get; set; }

    /// <summary>部屋調整タイプ</summary>
    [XmlAttribute("adjust_type")]
    [JsonPropertyName("adjust_type")]
    public EnumAdjustType AdjustType { get; set; } = EnumAdjustType.None;

    /// <summary>調整部屋数</summary>
    [XmlAttribute("rooms")]
    [JsonPropertyName("rooms")]
    public long RoomCount { get; set; }

    /// <summary>開始日</summary>
    [XmlAttribute("fromday")]
    [JsonPropertyName("fromday")]
    public string FromDay { get; set; } = string.Empty;

    /// <summary>対象日数</summary>
    [XmlAttribute("days")]
    [JsonPropertyName("days")]
    public long Days { get; set; }

    /// <summary>対象日一覧</summary>
    [XmlAttribute("dates")]
    [JsonPropertyName("dates")]
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
        [XmlEnum("")]
        [JsonPropertyName("")]
        None = 0,

        /// <summary>
        /// 相対部屋数指定：部屋数(rooms)を減らす
        /// </summary>
        [XmlEnum("1")]
        [JsonPropertyName("1")]
        RelativeDown = 1,

        /// <summary>
        /// 相対部屋数指定：部屋数(rooms)を増やす
        /// </summary>
        [XmlEnum("2")]
        [JsonPropertyName("2")]
        RelativeUp = 2,

        /// <summary>
        /// 絶対部屋数：部屋数(rooms)に設定する
        /// </summary>
        [XmlEnum("3")]
        [JsonPropertyName("3")]
        Absolute = 3,

        /// <summary>
        /// 売止 手仕舞いに設定する　※部屋数(rooms)を使用しない
        /// </summary>
        [XmlEnum("4")]
        [JsonPropertyName("4")]
        Stop = 4,

        /// <summary>
        /// 販売 手仕舞いを解除する　※部屋数(rooms)を使用しない
        /// </summary>
        [XmlEnum("5")]
        [JsonPropertyName("5")]
        Sale = 5
    }
}
