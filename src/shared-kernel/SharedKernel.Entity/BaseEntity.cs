namespace SharedKernel.Entity;

public class BaseEntity : IBaseEntity
{
    public bool IsEnabled { get; set; }

    public bool IsVisible { get; set; } = true;

    public string? RecordMemo { get; set; }

    public bool IsDeleted { get; set; }

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
