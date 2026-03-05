using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

/// <summary>
/// ファックスサービス
/// </summary>
public class FaxService : EntityData
{
    public string? Name { get; set; }

    /// <summary>
    /// メール送信形式でファックス送信できるか？
    /// </summary>
    public bool IsMailFax { get; set; }

    /// <summary>
    /// ファックス番号を含ませるメール送信の場合に使用するフォーマットです
    /// ex : ファックス番号:0123456789へ送信
    /// 0123456789@xxxx.xxx ->{0}@xxxx.xxx
    /// </summary>
    public string? MailFormat { get; set; }

    /// <summary>
    /// 単価
    /// </summary>
    public float UnitPrice { get; set; }

    /// <summary>
    /// 施設ファックスサービスリレーション
    /// </summary>
    public ICollection<FacilityFaxService>? FacilityFaxServices { get; set; }
}
