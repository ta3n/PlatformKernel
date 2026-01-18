using System.Xml.Serialization;
using static Liberty.Reservation.Application.Models.Responses.HotelModel;
using Newtonsoft.Json;
using System.ComponentModel;

namespace Liberty.Reservation.Application.Models.Responses;

[XmlRoot("response")]
public class SetRoomsResponse
{
    [XmlElement("result")]
    [JsonProperty("result")]
    public EnumResultType Result { get; set; }

    [XmlElement("reason")]
    [JsonProperty("reason")]
    public List<Reason> Reasons { get; set; } = [];

    [XmlElement("code")]
    [JsonProperty("code")]
    public string? Code { get; set; }

    public class Reason
    {
        [XmlAttribute("message")]
        [JsonProperty("message")]
        public FailureReason Message { get; set; }

        [XmlElement("hotel")]
        [JsonProperty("hotel")]
        public List<HotelModel> Hotels { get; set; } = [];
    }

    public enum EnumResultType
    {
        /// <summary>成功</summary>
        [XmlEnum("更新完了。")]
        [JsonProperty("更新完了。")]
        Success,

        /// <summary>失敗（部分に構文エラー、または矛盾な値になった可能性が高い）</summary>
        [XmlEnum("更新失敗。")]
        [JsonProperty("更新失敗。")]
        Failure,

        /// <summary>失敗（サーバー内、またはシステム側のエラーになった可能性が高い）</summary>
        [XmlEnum("サーバーエラー。")]
        [JsonProperty("サーバーエラー。")]
        Error,

        [XmlEnum("更新失敗。")]
        [JsonProperty("更新失敗。")]
        UpdateFailed,

        [XmlEnum("更新失敗。")]
        [JsonProperty("更新失敗。")]
        Processing
    }

    public class SetRoomModel
    {
        /// <summary>更新可能か</summary>
        public bool CanUpdate { get; set; } = true;

        /// <summary>更新対象施設リスト</summary>
        public List<Hotel> Hotels { get; set; } = [];

        /// <summary>更新対象施設</summary>
        public class Hotel(
            HotelModel hotel,
            List<long> dates
        )
        {
            /// <summary>
            /// key
            /// requestとresponseを紐づける一意な値
            /// </summary>
            public long Key { get; set; } = hotel.Key;

            /// <summary>施設番号(Code)</summary>
            public string? HotelId { get; set; } = hotel.HotelId;

            /// <summary>部屋番号(Code)</summary>
            public string? RoomId { get; set; } = hotel.RoomId;

            /// <summary>部屋調整タイプ</summary>
            public EnumAdjustType? AdjustType { get; set; } = hotel.AdjustType;

            /// <summary>調整部屋数</summary>
            public long RoomCount { get; set; } = hotel.RoomCount;

            /// <summary>対象日一覧</summary>
            public List<long> Dates { get; set; } = dates;

            /// <summary>更新失敗理由</summary>
            public FailureReason FailureReason { get; set; }
        }
    }

    public enum FailureReason
    {
        /// <summary>正常</summary>
        None,

        /// <summary>パラメータ不正(hotel 最大件数超過)</summary>
        [XmlEnum("パラメータ不正(hotel 最大件数超過)")]
        [JsonProperty("パラメータ不正(hotel 最大件数超過)")]
        [Description("パラメータ不正(hotel 最大件数超過)")]
        InvalidParameterExcessHotels,

        [XmlEnum("パラメータ不正(hotel_id 範囲外)")]
        [JsonProperty("パラメータ不正(hotel_id 範囲外)")]
        [Description("パラメータ不正(hotel_id 範囲外)")]
        InvalidParameterOutOfRangeHotelId,

        [XmlEnum("パラメータ不正(room_id 範囲外)")]
        [JsonProperty("パラメータ不正(room_id 範囲外)")]
        [Description("パラメータ不正(room_id 範囲外)")]
        InvalidParameterOutOfRangeRoomId,

        [XmlEnum("パラメータ不正(rooms 範囲外)")]
        [JsonProperty("パラメータ不正(rooms 範囲外)")]
        [Description("パラメータ不正(rooms 範囲外)")]
        InvalidParameterOutOfRangeRoomCount,

        [XmlEnum("パラメータ不正(adjust_type 範囲外)")]
        [JsonProperty("パラメータ不正(adjust_type 範囲外)")]
        [Description("パラメータ不正(adjust_type 範囲外)")]
        InvalidParameterOutOfRangeAdjustType,

        [XmlEnum("パラメータ不正(fromday,dates 両方未指定)")]
        [JsonProperty("パラメータ不正(fromday,dates 両方未指定)")]
        [Description("パラメータ不正(fromday,dates 両方未指定)")]
        InvalidParameterNoUseDateParams,

        [XmlEnum("パラメータ不正(fromday,dates 両方指定)")]
        [JsonProperty("パラメータ不正(fromday,dates 両方指定)")]
        [Description("パラメータ不正(fromday,dates 両方指定)")]
        InvalidParameterUseAllDateParams,

        [XmlEnum("パラメータ不正(fromday 日付フォーマット不正)")]
        [JsonProperty("パラメータ不正(fromday 日付フォーマット不正)")]
        [Description("パラメータ不正(fromday 日付フォーマット不正)")]
        InvalidParameterFormatFromday,

        [XmlEnum("パラメータ不正(fromday 前日以前)")]
        [JsonProperty("パラメータ不正(fromday 前日以前)")]
        [Description("パラメータ不正(fromday 前日以前)")]
        InvalidParameterOldFromday,

        [XmlEnum("パラメータ不正(days 範囲外)")]
        [JsonProperty("パラメータ不正(days 範囲外)")]
        [Description("パラメータ不正(days 範囲外)")]
        InvalidParameterOutOfRangeDays,

        [XmlEnum("パラメータ不正(dates 日付フォーマット不正)")]
        [JsonProperty("パラメータ不正(dates 日付フォーマット不正)")]
        [Description("パラメータ不正(dates 日付フォーマット不正)")]
        InvalidParameterFormatDates,

        [XmlEnum("パラメータ不正(dates 前日以前)")]
        [JsonProperty("パラメータ不正(dates 前日以前)")]
        [Description("パラメータ不正(dates 前日以前)")]
        InvalidParameterOldDates,

        [XmlEnum("パラメータ不正(dates 重複)")]
        [JsonProperty("パラメータ不正(dates 重複)")]
        [Description("パラメータ不正(dates 重複)")]
        InvalidParameterDuplicationDates,

        [XmlEnum("パラメータ不正(dates 最大件数超過)")]
        [JsonProperty("パラメータ不正(dates 最大件数超過)")]
        [Description("パラメータ不正(dates 最大件数超過)")]
        InvalidParameterExcessDates,

        [XmlEnum("データ矛盾(部屋在庫数マイナス)")]
        [JsonProperty("データ矛盾(部屋在庫数マイナス)")]
        [Description("データ矛盾(部屋在庫数マイナス)")]
        MinusSellNumber,

        [XmlEnum("データ矛盾（最大数超過)")]
        [JsonProperty("データ矛盾（最大数超過)")]
        [Description("データ矛盾（最大数超過)")]
        OverBaseNumber,

        [XmlEnum("データ矛盾(部屋在庫未設定期間)")]
        [JsonProperty("データ矛盾(部屋在庫未設定期間)")]
        [Description("データ矛盾(部屋在庫未設定期間)")]
        UnsetPeriodSellNumber,

        [XmlEnum("パラメータ重複")]
        [JsonProperty("パラメータ重複")]
        [Description("パラメータ重複")]
        HasDuplicateInput
    }
}
