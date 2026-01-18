using System.ComponentModel.DataAnnotations.Schema;
using Liberty.Entity;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Newtonsoft.Json;

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
    public MultilingualText? Name { get; set; }

    /// <summary>
    /// 補足
    /// </summary>
    public MultilingualText? Description { get; set; }

    /// <summary>
    /// 必須回答か？
    /// </summary>
    public bool IsRequired { get; set; }

    public MultilingualText? FormData { get; set; }

    /// <summary>
    /// 施設質問リレーション
    /// </summary>
    public ICollection<FacilityQuestion>? FacilityQuestions { get; set; }

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

    [NotMapped]
    public bool HasContent
    {
        get
        {
            if (QuestionType == QuestionTypes.Unknown)
            {
                return false;
            }

            var content = FormData?.GetValueByHeader();
            if (string.IsNullOrEmpty(content))
            {
                return false;
            }

            var contentData = JsonConvert.DeserializeObject<QuestionFormData>(content);
            if (contentData == null || string.IsNullOrEmpty(contentData.Type))
            {
                return false;
            }

            return QuestionType switch
            {
                QuestionTypes.Text or QuestionTypes.Textarea => true,
                QuestionTypes.Select or QuestionTypes.Radio or QuestionTypes.Checkbox => contentData.Data?.Count > 0,
                _ => false
            };
        }
        set => _ = value;
    }
}
