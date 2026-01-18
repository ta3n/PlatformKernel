using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Liberty.Entity;

/// <summary>
/// 全エンティティの共通情報および基底処理クラス
/// </summary>
//public abstract class BaseData<U> : IBaseData<U>, ISorter
//public abstract class BaseData<U> : ISorter
public class EntityData : BaseEntity, IEntityData
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    /// <summary>
    /// コード
    /// </summary>
    public string? Code { get; set; }

    public virtual void Delete()
    {
        IsDeleted = true;
    }
}
