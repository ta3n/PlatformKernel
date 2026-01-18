using Liberty.Entity;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

/// <summary>
/// 質問事項
/// </summary>
public class Question : EntityData
{
    /// <summary>
    /// 質問種別
    /// ラジオボタン、チェックボックス・・・
    /// </summary>
    public QuestionTypes QuestionType { get; set; }

    /// <summary>
    /// 質問事項名
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 補足
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 必須回答か？
    /// </summary>
    public bool IsRequired { get; set; }

    public string? FormData { get; set; }

    /// <summary>
    /// 施設質問リレーション
    /// </summary>
    public ICollection<FacilityQuestion>? FacilityQuestions { get; set; }

    /// <summary>
    /// オプションアイテム質問リレーション
    /// </summary>
    public ICollection<OptionItemQuestion>? OptionItemQuestion { get; set; }

    /// <summary>
    /// プラン利用時の予約者への質問リレーション
    /// </summary>
    public ICollection<PlanQuestion>? PlanQuestions { get; set; }

    /// <summary>
    /// オプションアイテム質問リレーション
    /// </summary>
    public ICollection<OptionItemQuestion>? OptionItemQuestions { get; set; }

    /// <summary>
    /// 予約・質問回答リレーション
    /// </summary>
    public ICollection<ReservationQuestion>? ReservationQuestions { get; set; }
}
