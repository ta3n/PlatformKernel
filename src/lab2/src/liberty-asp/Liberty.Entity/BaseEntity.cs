namespace Liberty.Entity;

public class BaseEntity : IBaseEntity
{
    /// <summary>
    /// 有効か？
    /// </summary>
    public bool IsEnabled { get; set; } = false;

    /// <summary>
    /// 公開・非公開
    /// </summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// レコードメモ
    /// </summary>
    public string? RecordMemo { get; set; }

    /// <summary>
    /// 論理削除フラグ
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// 並び順
    /// </summary>
    public long DisplayOrder { get; set; }

    public long CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public long? UpdatedAt { get; set; }

    public string? DeletedBy { get; set; }

    public long? DeletedAt { get; set; }

    public object Clone()
    {
        return MemberwiseClone();
    }

    public T Clone<T>()
    {
        return (T)MemberwiseClone();
    }
}
