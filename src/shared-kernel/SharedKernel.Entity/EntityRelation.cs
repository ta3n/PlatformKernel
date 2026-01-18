namespace SharedKernel.Entity;

public class EntityRelation : BaseEntity, IEntityRelation
{
    public new string? RecordMemo { get; set; }

    protected EntityRelation()
    {
        IsEnabled = true;
    }
}
