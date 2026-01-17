namespace SharedKernel.Entity;

/// <summary>
/// 全エンティティの共通情報および基底処理クラス
/// </summary>
public class EntityRelation : BaseEntity, IEntityRelation
{
    /// <summary>
    /// レコードメモ
    /// </summary>
    public new string? RecordMemo { get; set; }

    protected EntityRelation()
    {
        IsEnabled = true;
    }
}
