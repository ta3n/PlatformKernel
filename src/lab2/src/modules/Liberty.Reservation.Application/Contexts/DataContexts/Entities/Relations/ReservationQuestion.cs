using Liberty.Entity;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

public class ReservationQuestion : EntityRelation
{
    /// <summary>
    /// 質問事項ID
    /// </summary>
    public long ReservationId { get; set; }

    /// <summary>
    /// 質問事項名
    /// </summary>
    public Data.Reservation? Reservation { get; set; }

    /// <summary>
    /// 質問事項ID
    /// </summary>
    public long QuestionId { get; set; }

    /// <summary>
    /// 質問事項名
    /// </summary>
    public Question? Question { get; set; }

    /// <summary>
    /// 保存時のシリアライズしたQuestion文字列
    /// </summary>
    public string? QuestionObj { get; set; }

    /// <summary>
    /// 質問分類(施設からのお知らせか、オプションについてか？
    /// </summary>
    public ReservationQuestionTypes ReservationQuestionType { get; set; }

    /// <summary>
    /// 回答データ
    /// </summary>
    public string? AnswerData { get; set; }

    /// <summary>
    /// 必須回答かどうか
    /// </summary>
    public bool IsRequired { get; set; }
}
