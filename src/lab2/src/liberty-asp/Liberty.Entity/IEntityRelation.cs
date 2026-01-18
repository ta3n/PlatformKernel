namespace Liberty.Entity;

public interface IEntityRelation : IBaseEntity
{
    new string? RecordMemo { get; set; }
}
