using Liberty.Entity;
using Liberty.Reservation.Application.Constants;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

/// <summary>
/// ユーザ情報
/// </summary>
public class CustomerInfo : EntityData
{
    /// <summary>
    /// 名前
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// かな
    /// </summary>
    public string? Kana { get; set; }

    /// <summary>
    /// メールアドレス
    /// </summary>
    public string? EMail { get; set; }

    /// <summary>
    /// 英語表記名
    /// </summary>
    public string? NameE { get; set; }

    /// <summary>
    /// 性別
    /// </summary>
    public Genders Gender { get; set; }

    public long? CountryId { get; set; }

    /// <summary>
    /// 国ID
    /// </summary>
    public Country? Country { get; set; }

    public string? CountryCode { get; set; }

    /// <summary>
    /// 郵便番号
    /// </summary>
    public string? PostCode { get; set; }

    /// <summary>
    /// 住所
    /// </summary>
    public string? Address1 { get; set; }

    public string? Address2 { get; set; }

    public string? Address3 { get; set; }

    /// <summary>
    /// 電話番号
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 誕生日
    /// </summary>
    public long? BirthDay { get; set; }
}
